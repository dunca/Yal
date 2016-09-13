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

namespace Yal
{
    static class FileManager
    {
        internal const string indexDb = "index.sqlite";
        internal const string historyDb = "history.sqlite";
        private static string queryTemplate = @"select distinct name, fullpath from 
                                                (select NAME, FULLPATH, HITS as STATIC from HISTORY where SNIPPET like @snip
                                                union 
                                                select NAME, FULLPATH, 0 as STATIC from CATALOG where NAME like @query 
                                                order by STATIC desc, NAME asc) limit @limit";
        private static string insertTemplate = "insert into CATALOG (NAME, FULLPATH) values (@name, @fullpath)";

        private static IEnumerable<string> Search(string path, string pattern = "exe,lnk", 
                                                  SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            string[] patterns = pattern.Split(',');
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file => patterns.Any(p => file.EndsWith(string.Concat(".", p))));
        }

        private static void UpdateIndex(IEnumerable<string> files)
        {
            using (var connection = GetDbConnection())
            {
                foreach (string file in files)
                {
                    if (file.Length > 260)
                    {
                        continue;
                    }
                    var command = new SQLiteCommand(insertTemplate, connection);
                    command.Parameters.AddWithValue("@name", Path.GetFileName(file));
                    command.Parameters.AddWithValue("@fullpath", file);
                    command.ExecuteNonQuery();
                }
            }
        }

        internal static void UpdateHistory(string snippet, string fileName, string fullPath)
        {
            using (var connection = GetDbConnection(historyDb))
            {
                SQLiteCommand nonQuery;
                var command = new SQLiteCommand($"select count(snippet) from HISTORY where SNIPPET == @snippet and FULLPATH == @fullpath",
                                                connection);
                command.Parameters.AddWithValue("@snippet", snippet);
                command.Parameters.AddWithValue("@fullpath", fullPath);
                if (Convert.ToBoolean(command.ExecuteScalar()))  // snippet + fileName combo already in DB
                { // update the existing value
                    nonQuery = new SQLiteCommand($"update HISTORY set HITS = HITS + 1, LASTACCESSED = datetime('now') where SNIPPET == @snippet and FULLPATH == @fullpath",
                                                 connection);
                }
                else
                {
                    nonQuery = new SQLiteCommand($"insert into HISTORY (SNIPPET, NAME, FULLPATH, LASTACCESSED) values (@snippet, @name, @fullpath, datetime('now'))",
                                                 connection);
                    nonQuery.Parameters.AddWithValue("@name", fileName);
                }
                nonQuery.Parameters.AddWithValue("@snippet", snippet);
                nonQuery.Parameters.AddWithValue("@fullpath", fullPath);
                nonQuery.ExecuteNonQuery();
            }
        }

        private static void ClearDB(string dbName = indexDb)
        {
            using (var connection = GetDbConnection(dbName))
            {
                var table = dbName == indexDb ? "CATALOG" : "HISTORY";
                (new SQLiteCommand($"delete from {table}", connection)).ExecuteNonQuery();
            }
        }

        internal static bool RemoveFromDb(string fullPath, string dbName)
        {
            using (var connection = GetDbConnection(dbName))
            {
                var command = new SQLiteCommand($"delete from {(dbName == indexDb ? "CATALOG" : "HISTORY")} where FULLPATH == @fullpath", connection);
                command.Parameters.AddWithValue("@fullpath", fullPath);
                return Convert.ToBoolean(command.ExecuteNonQuery());
            }
        }

        private static void CreateDatabase(string dbName = indexDb)
        {
            SQLiteConnection.CreateFile(dbName);
            using (var connection = GetDbConnection(dbName))
            {
                SQLiteCommand command;
                if (dbName == indexDb)
                {
                    command = new SQLiteCommand("create table CATALOG (NAME string, FULLPATH string)", connection);
                }
                else
                {
                    command = new SQLiteCommand("create table HISTORY (SNIPPET string, NAME string, FULLPATH string, HITS integer default 1, LASTACCESSED datetime)", connection);
                }
                command.ExecuteNonQuery();
            }
        }

        private static SQLiteConnection GetDbConnection(string dbName = indexDb)
        {
            var connection = new SQLiteConnection($"Data Source={dbName};Version=3;");
            connection.Open();
            return connection;
        }

        internal static bool QueryIndexDb(string partialFileName, int fetchLimit, ListView.ListViewItemCollection items, 
                                          ImageList.ImageCollection images)
        {
            using (var connection = GetDbConnection())
            {
                (new SQLiteCommand($"ATTACH '{historyDb}' as HISTORY", connection)).ExecuteNonQuery();

                var command = new SQLiteCommand(queryTemplate, connection);
                var query = string.Concat(Properties.Settings.Default.MatchAnywhere ? "%" : "", partialFileName, "%");
                command.Parameters.AddWithValue("@query", query);
                command.Parameters.AddWithValue("@limit", fetchLimit);
                command.Parameters.AddWithValue("@snip", string.Concat(partialFileName, "%"));
                SQLiteDataReader response = command.ExecuteReader();

                int iconIndex = images.Count; // plugins could also have images, so we can't start form 0
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
                                                        imageIndex: iconIndex)
                        { ToolTipText = fullPath };
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

            ClearDB();
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

        internal static int DbRowCount(string dbName = indexDb)
        {
            using (var connection = GetDbConnection(dbName))
            {
                var command = new SQLiteCommand($"select count(name) from {(dbName == indexDb ? "CATALOG" : "HISTORY")}", connection);
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
            int oldStop = -1;
            newPath = oldPath;
            var vars = new List<string>();

            while (true)
            {
                int start = oldPath.IndexOf("%", oldStop + 1);
                int stop = oldPath.IndexOf("%", start + 1);
                if (start < 0 || stop < 0)
                {
                    break;
                }
                // copy just the variable, without the starting and ending '%'
                vars.Add(oldPath.Substring(start + 1, stop - start - 1));
                oldStop = stop;
            }

            foreach (var variable in vars)
            {
                newPath = oldPath.Replace($"%{variable}%", Environment.GetEnvironmentVariable(variable));
            }

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
            using (var connection = GetDbConnection(historyDb))
            {
                int historySize = DbRowCount(historyDb);
                if ( historySize > Properties.Settings.Default.MaxHistorySize)
                {
                    var command = new SQLiteCommand("delete from HISTORY where LASTACCESSED in (select LASTACCESSED from HISTORY order by LASTACCESSED limit @limit)", connection);
                    command.Parameters.AddWithValue("@limit", historySize - Properties.Settings.Default.MaxHistorySize);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
