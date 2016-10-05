using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;

using Utilities;

namespace Yal
{
    struct DbInfo
    {
        public readonly string fileName;
        public readonly string tableName;
        public readonly string mainColumn;
        public readonly string createTableTemplate;

        public DbInfo(string file, string table, string mainCol, string tableTemplate)
        {
            fileName = file;
            tableName = table;
            mainColumn = mainCol;
            createTableTemplate = tableTemplate;
        }
    }

    static class FileManager
    {
        internal static DbInfo indexDbInfo = new DbInfo("index.sqlite", "INDEX_CATALOG", "FULLPATH",
                                                        "create table if not exists INDEX_CATALOG (NAME text, FULLPATH text)");
        internal static DbInfo historyDbInfo = new DbInfo("history.sqlite", "HISTORY", "SUBITEM",
                                                          "create table if not exists HISTORY_CATALOG (SNIPPET text, ITEM text, SUBITEM text, ITEM_INFO text, PLUGIN_NAME text, ICON_PATH text, HITS integer default 1, LASTACCESSED datetime)");

        private const string indexInsert = "insert into INDEX_CATALOG (NAME, FULLPATH) values (@name, @fullpath)";
        private const string historyInsert = "insert into HISTORY_CATALOG (SNIPPET, ITEM, SUBITEM, ITEM_INFO, PLUGIN_NAME, ICON_PATH, LASTACCESSED) values (@snippet, @item, @subitem, @item_info, @plugin_name, @icon_path, datetime('now'))";
        private const string historyTrim = "delete from HISTORY_CATALOG where LASTACCESSED in (select LASTACCESSED from HISTORY_CATALOG order by LASTACCESSED limit @limit)";
        private const string historyUpdate = "update HISTORY_CATALOG set HITS = HITS + 1, LASTACCESSED = datetime('now') where SNIPPET = @snippet and SUBITEM = @subitem";
        private const string historyQuery = "select count(SNIPPET) from HISTORY_CATALOG where SNIPPET = @snippet and SUBITEM == @subitem";

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

        internal static void UpdateHistory(string snippet, string item, string subitem, string itemInfo = "", string pluginName = "", string iconPath = "")
        {
            using (var connection = GetDbConnection(historyDbInfo))
            {
                SQLiteCommand nonQuery;
                var command = new SQLiteCommand(historyQuery, connection);
                command.Parameters.AddWithValue("@snippet", snippet);
                command.Parameters.AddWithValue("@subitem", subitem);

                if (Convert.ToBoolean(command.ExecuteScalar()))  // snippet + fileName combo already in DB
                { // update the existing value
                    nonQuery = new SQLiteCommand(historyUpdate, connection);
                }
                else
                {
                    nonQuery = new SQLiteCommand(historyInsert, connection);
                    nonQuery.Parameters.AddWithValue("@item", item);
                }

                nonQuery.Parameters.AddWithValue("@snippet", snippet);
                nonQuery.Parameters.AddWithValue("@subitem", subitem);
                nonQuery.Parameters.AddWithValue("@item_info", itemInfo);
                nonQuery.Parameters.AddWithValue("@plugin_name", pluginName);
                nonQuery.Parameters.AddWithValue("@icon_path", iconPath);
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

        internal static bool RemoveFromDb(string itemIdentifier, DbInfo dbInfo)
        {
            using (var connection = GetDbConnection(dbInfo))
            {
                var command = new SQLiteCommand($"delete from {dbInfo.tableName} where {dbInfo.mainColumn} == @item", connection);
                command.Parameters.AddWithValue("@item", itemIdentifier);
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

        internal static SQLiteConnection GetDbConnection(DbInfo dbInfo)
        {
            var connection = new SQLiteConnection($"Data Source={dbInfo.fileName};Version=3;");
            connection.Open();
            return connection;
        }

        //internal static bool QueryIndexDb(string partialFileName, ListView.ListViewItemCollection items,
        //                                  ImageList.ImageCollection images)
        //{
        //    using (var connection = GetDbConnection(indexDbInfo))
        //    {
        //        (new SQLiteCommand($"ATTACH '{historyDbInfo.fileName}' as HISTORY", connection)).ExecuteNonQuery();

        //        var command = new SQLiteCommand(fileQuery, connection);
        //        string pattern = Properties.Settings.Default.FuzzyMatching ? string.Concat(partialFileName.Select(c => string.Concat(c, "%"))) :
        //                                                                     string.Concat(partialFileName, "%");
        //        var query = string.Concat(Properties.Settings.Default.MatchAnywhere ? "%" : "", pattern);
        //        command.Parameters.AddWithValue("@query", query);
        //        command.Parameters.AddWithValue("@limit", Properties.Settings.Default.MaxItems - items.Count);
        //        command.Parameters.AddWithValue("@snip", string.Concat(partialFileName, "%"));
        //        SQLiteDataReader response = command.ExecuteReader();

        //        int iconIndex = images.Count; // plugins could also have images (see Yal.cs/PerformSearch()), so we can't start form 0
        //        while (response.Read())
        //        {
        //            var name = response["NAME"].ToString();
        //            if (!Properties.Settings.Default.ExtensionInFileName)
        //            {
        //                name = Path.GetFileNameWithoutExtension(name);
        //            }
        //            var fullPath = response["FULLPATH"].ToString();
        //            Icon icon;
        //            if (GetFileIcon(fullPath, out icon))  // to avoid exceptions when the file is deleted but it's still in the db
        //            {
        //                images?.Add(icon);
        //                var listItem = new ListViewItem(new string[] { name, fullPath },
        //                                                imageIndex: iconIndex)
        //                { ToolTipText = fullPath };
        //                items.Add(listItem);
        //                iconIndex++;
        //            }
        //        }
        //    }
        //    return items.Count > 0;
        //}

        internal static bool GetFileIcon(string fullName, out Icon icon)
        {
            icon = null;
            if (File.Exists(fullName))
            {
                icon = Icon.ExtractAssociatedIcon(fullName);
            }
            return icon != null;
        }

        internal static Task RebuildIndexAsync()
        {
            return Task.Run(() => RebuildIndex());
        }

        private static void RebuildIndex()
        {
            ClearDB(indexDbInfo);

            var searchOption = Properties.Settings.Default.IncludeSubdirs ?
                               SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var extensions = Properties.Settings.Default.Extensions.Split(',').Select(ext => string.Concat(".", ext));

            var directoryStack = new Stack<string>();
            foreach (string directory in Properties.Settings.Default.FoldersToIndex)
            {
                // Convert.ToBoolean(null) -> false; So this will work even if FoldersToExclude is null;
                if (!Convert.ToBoolean(Properties.Settings.Default.FoldersToExclude?.Contains(directory)))
                {
                    directoryStack.Push(directory);
                }
            }

            IEnumerable<string> currentFiles;
            IEnumerable<string> currentSubdirs;
            var failedToIndex = new List<string>();

            while (directoryStack.Count > 0)
            {
                var currentDirectory = directoryStack.Pop();

                try
                {
                    currentSubdirs = Directory.EnumerateDirectories(currentDirectory);
                }
                catch (Exception ex)
                {
                    failedToIndex.Add(string.Join(" - ", currentDirectory, ex.Message));
                    continue;
                }

                currentFiles = Directory.EnumerateFiles(currentDirectory, "*.*").Where(file => extensions.Contains(Path.GetExtension(file)));

                UpdateIndex(currentFiles);

                foreach (var subdir in currentSubdirs)
                {
                    directoryStack.Push(subdir);
                }
            }

            Properties.Settings.Default.DateLastIndexed = DateTime.Now;

            if (failedToIndex.Count > 0)
            {
                MessageBox.Show($"The following directories were not indexed: \n{string.Join("\n", failedToIndex)}",
                                "Indexing finished", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

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
                var command = new SQLiteCommand($"select count(@column) from {dbInfo.tableName}", connection);
                command.Parameters.AddWithValue("@column", dbInfo.mainColumn);
                return Convert.ToInt32(command.ExecuteScalar());
            }
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
                if (Utils.LocalizePath(location, out result))
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
                if (historySize > Properties.Settings.Default.MaxHistorySize)
                {
                    var command = new SQLiteCommand(historyTrim, connection);
                    command.Parameters.AddWithValue("@limit", historySize - Properties.Settings.Default.MaxHistorySize);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}