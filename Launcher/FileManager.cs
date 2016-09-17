using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Yal
{
    struct DbInfo
    {
        public readonly string fileName;
        public readonly string tableName;
        public readonly string createTableTemplate;

        public DbInfo(string file, string table, string tableTemplate)
        {
            fileName = file;
            tableName = table;
            createTableTemplate = tableTemplate;
        }
    }

    static class FileManager
    {
        internal static DbInfo indexDbInfo = new DbInfo("index.sqlite", "CATALOG", 
                                                        "create table if not exists CATALOG (NAME string, FULLPATH string)");
        internal static DbInfo historyDbInfo = new DbInfo("history.sqlite", "HISTORY", 
                                                          "create table if not exists HISTORY (SNIPPET string, NAME string, FULLPATH string, HITS integer default 1, LASTACCESSED datetime)");

        private const string fileQuery = @"select distinct name, fullpath from 
                                         (select NAME, FULLPATH, HITS as STATIC from HISTORY where SNIPPET like @snip
                                         union 
                                         select NAME, FULLPATH, 0 as STATIC from CATALOG where NAME like @query 
                                         order by STATIC desc, NAME asc) limit @limit";

        // used to replace environment variable placeholders inside the app's default indexing paths, with their actual values
        private static Regex envVarRegex = new Regex(@"%([\w\d]+)%");

        private const string indexInsert = "insert into CATALOG (NAME, FULLPATH) values (@name, @fullpath)";
        private const string historyInsert = "insert into HISTORY (SNIPPET, NAME, FULLPATH, LASTACCESSED) values (@snippet, @name, @fullpath, datetime('now'))";
        private const string historyTrim = "delete from HISTORY where LASTACCESSED in (select LASTACCESSED from HISTORY order by LASTACCESSED limit @limit)";
        private const string historyUpdate = "update HISTORY set HITS = HITS + 1, LASTACCESSED = datetime('now') where SNIPPET == @snippet and FULLPATH == @fullpath";
        private const string historyQuery = "select count(snippet) from HISTORY where SNIPPET == @snippet and FULLPATH == @fullpath";
        
        private static IEnumerable<string> Search(string path, string pattern, 
                                                  SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            string[] patterns = pattern.Split(',');
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file => patterns.Any(p => file.EndsWith(string.Concat(".", p))));
        }

        private static void UpdateIndex(IEnumerable<string> files)
        {
            using (var connection = GetDbConnection(indexDbInfo))
            {
                foreach (string file in files)
                {
                    // it seems that Path.GetFileName() and other methods can't deal with files that have really long paths/names
                    if (file.Length > 260)
                    {
                        continue;
                    }
                    var command = new SQLiteCommand(indexInsert, connection);
                    command.Parameters.AddWithValue("@name", Path.GetFileName(file));
                    command.Parameters.AddWithValue("@fullpath", file);
                    command.ExecuteNonQuery();
                }
            }
        }

        internal static void UpdateHistory(string snippet, string fileName, string fullPath)
        {
            using (var connection = GetDbConnection(historyDbInfo))
            {
                SQLiteCommand nonQuery;
                var command = new SQLiteCommand(historyQuery, connection);
                command.Parameters.AddWithValue("@snippet", snippet);
                command.Parameters.AddWithValue("@fullpath", fullPath);
                if (Convert.ToBoolean(command.ExecuteScalar()))  // snippet + fileName combo already in DB
                { // update the existing value
                    nonQuery = new SQLiteCommand(historyUpdate, connection);
                }
                else
                {
                    nonQuery = new SQLiteCommand(historyInsert, connection);
                    nonQuery.Parameters.AddWithValue("@name", fileName);
                }
                nonQuery.Parameters.AddWithValue("@snippet", snippet);
                nonQuery.Parameters.AddWithValue("@fullpath", fullPath);
                nonQuery.ExecuteNonQuery();
            }
        }

        private static void ClearDB(DbInfo dbInfo)
        {
            using (var connection = GetDbConnection(dbInfo))
            {
                (new SQLiteCommand($"delete from {dbInfo.tableName}", connection)).ExecuteNonQuery();
            }
        }

        internal static bool RemoveFromDb(string fullPath, DbInfo dbInfo)
        {
            using (var connection = GetDbConnection(dbInfo))
            {
                var command = new SQLiteCommand($"delete from {dbInfo.tableName} where FULLPATH == @fullpath", connection);
                command.Parameters.AddWithValue("@fullpath", fullPath);
                return Convert.ToBoolean(command.ExecuteNonQuery());
            }
        }

        internal static void EnsureDbExists(out bool didNotExist)
        {
            CreateDatabase(historyDbInfo);
            didNotExist = !Convert.ToBoolean(CreateDatabase(indexDbInfo));
        }

        private static int CreateDatabase(DbInfo dbInfo)
        {
            using (var connection = GetDbConnection(dbInfo))
            {
                // -1 if table already existed, 0 if it was created
                return (new SQLiteCommand(dbInfo.createTableTemplate, connection)).ExecuteNonQuery(); ;
            }
        }

        private static SQLiteConnection GetDbConnection(DbInfo dbInfo)
        {
            var connection = new SQLiteConnection($"Data Source={dbInfo.fileName};Version=3;");
            connection.Open();
            return connection;
        }

        internal static bool QueryIndexDb(string partialFileName, ListView.ListViewItemCollection items, 
                                          ImageList.ImageCollection images)
        {
            using (var connection = GetDbConnection(indexDbInfo))
            {
                (new SQLiteCommand($"ATTACH '{historyDbInfo.fileName}' as HISTORY", connection)).ExecuteNonQuery();

                var command = new SQLiteCommand(fileQuery, connection);
                string pattern = Properties.Settings.Default.FuzzyMatching ? string.Concat(partialFileName.Select(c => string.Concat(c, "%"))) : 
                                                                             string.Concat(partialFileName, "%");
                var query = string.Concat(Properties.Settings.Default.MatchAnywhere ? "%" : "", pattern);
                command.Parameters.AddWithValue("@query", query);
                command.Parameters.AddWithValue("@limit", Properties.Settings.Default.MaxItems - items.Count);
                command.Parameters.AddWithValue("@snip", string.Concat(partialFileName, "%"));
                SQLiteDataReader response = command.ExecuteReader();

                int iconIndex = images.Count; // plugins could also have images (see Yal.cs/PerformSearch()), so we can't start form 0
                while (response.Read())
                {
                    var name = response["NAME"].ToString();
                    if (!Properties.Settings.Default.ExtensionInFileName)
                    {
                        name = Path.GetFileNameWithoutExtension(name);
                    }
                    var fullPath = response["FULLPATH"].ToString();
                    Icon icon;
                    if (GetFileIcon(fullPath, out icon))  // to avoid exceptions when the file is deleted but it's still in the db
                    {
                        images?.Add(icon);
                        var listItem = new ListViewItem(new string[] { name, fullPath },
                                                        imageIndex: iconIndex) { ToolTipText = fullPath };
                        items.Add(listItem);
                        iconIndex++;
                    }
                }
            }
            return items.Count > 0;
        }

        private static bool GetFileIcon(string fullName, out Icon icon)
        {
            if (File.Exists(fullName))
            {
                icon =  Icon.ExtractAssociatedIcon(fullName);
                return true;
            }
            icon = null;
            return false;
        }

        internal static void RebuildIndex()
        {
            var searchOption = Properties.Settings.Default.Subdirectories ?
                               SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            ClearDB(indexDbInfo);
            foreach (string directory in Properties.Settings.Default.FoldersToIndex)
            {
                // Convert.ToBoolean(null) -> false; So this will work even if FoldersToExclude is null;
                if (!Convert.ToBoolean(Properties.Settings.Default.FoldersToExclude?.Contains(directory)))
                {
                    UpdateIndex(Search(directory, Properties.Settings.Default.Extensions, searchOption));
                }
            }
            Properties.Settings.Default.DateLastIndexed = DateTime.Now;

            //// remove items for history that are no longer in the index
            //using (var connection = GetDbConnection(historyDb))
            //{
            //    (new SQLiteCommand($"ATTACH '{indexDb}' as CATALOG", connection)).ExecuteNonQuery();
            //    var command = new SQLiteCommand("delete from HISTORY where FULLPATH no in (select FULLPATH FROM CATALOG)", connection);
            //    command.ExecuteNonQuery();
            //}
        }

        internal static int DbRowCount(DbInfo dbInfo)
        {
            using (var connection = GetDbConnection(dbInfo))
            {
                var command = new SQLiteCommand($"select count(NAME) from {dbInfo.tableName}", connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// This method replaces environment variable placeholders with the actual variables
        /// </summary>
        /// <param name="oldPath">Eg. C:\some\cute\dir\%username%\nice</param>
        /// <param name="newPath">Eg. C:\some\cute\dir\Johnny\nice</param>
        /// <returns></returns>
        private static bool PersonalizePath(string oldPath, out string newPath)
        {
            
            // newPath = oldPath;
            
            // 1st try
            //int oldStop = -1;
            //var vars = new List<string>();
            //while (true)
            //{
            //    int start = oldPath.IndexOf("%", oldStop + 1);
            //    int stop = oldPath.IndexOf("%", start + 1);
            //    if (start < 0 || stop < 0)
            //    {
            //        break;
            //    }
            //    // copy just the variable, without the starting and ending '%'
            //    vars.Add(oldPath.Substring(start + 1, stop - start - 1));
            //    oldStop = stop;
            //}

            // 2nd try
            //foreach (Match match in envVarRegex.Matches(oldPath))
            //{
            //    string envVar = match.Groups[1].Value;
            //    newPath = oldPath.Replace($"%{envVar}%", Environment.GetEnvironmentVariable(envVar));
            //}
            newPath = envVarRegex.Replace(oldPath, match => Environment.GetEnvironmentVariable(match.Groups[1].Value));

            if (Directory.Exists(newPath))
            {
                return true;
            }

            newPath = null;
            return false;
        }

        /// <summary>
        /// Makes sure all paths used for indexing are valid by replacing environment variable placeholders with their actual values
        /// </summary>
        /// <returns></returns>
        internal static StringCollection ProcessRawPaths()
        {
            var sc = new StringCollection();
            foreach (string location in Properties.Settings.Default.FoldersToIndex)
            {
                string result = null;
                if (PersonalizePath(location, out result))
                {
                    sc.Add(result);
                }
            }
            return sc;
        }

        internal static void TrimHistory()
        {
            using (var connection = GetDbConnection(historyDbInfo))
            {
                int historySize = DbRowCount(historyDbInfo);
                if ( historySize > Properties.Settings.Default.MaxHistorySize)
                {
                    var command = new SQLiteCommand(historyTrim, connection);
                    command.Parameters.AddWithValue("@limit", historySize - Properties.Settings.Default.MaxHistorySize);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
