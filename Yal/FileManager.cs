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
        internal static DbInfo historyDbInfo = new DbInfo("history.sqlite", "HISTORY_CATALOG", "SUBITEM",
                                                          "create table if not exists HISTORY_CATALOG (SNIPPET text, ITEM text, SUBITEM text, ITEM_INFO text, PLUGIN_NAME text, ICON_PATH text, HITS integer default 1, LASTACCESSED datetime)");

        private static string indexInsert = $"insert into {indexDbInfo.tableName} (NAME, FULLPATH) values (@name, @fullpath)";
        private static string historyInsert = $"insert into {historyDbInfo.tableName} (SNIPPET, ITEM, SUBITEM, ITEM_INFO, PLUGIN_NAME, ICON_PATH, LASTACCESSED) values (@snippet, @item, @subitem, @item_info, @plugin_name, @icon_path, datetime('now'))";
        private static string historyTrim = $"delete from {historyDbInfo.tableName} where LASTACCESSED in (select LASTACCESSED from {historyDbInfo.tableName} order by LASTACCESSED limit @limit)";
        private static string historyUpdate = $"update {historyDbInfo.tableName} set HITS = HITS + 1, ICON_PATH = @icon_path, LASTACCESSED = datetime('now') where SNIPPET = @snippet and case PLUGIN_NAME when '' then SUBITEM = @subitem else ITEM_INFO = @item_info end";
        private static string historyQuery = $"select count(SNIPPET) from {historyDbInfo.tableName} where SNIPPET = @snippet and case PLUGIN_NAME when '' then SUBITEM = @subitem else ITEM_INFO = @item_info end";

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
                command.Parameters.AddWithValue("@item_info", itemInfo);

                if (Convert.ToBoolean(command.ExecuteScalar()))  // snippet + fileName combo already in DB
                { // update the existing value
                    nonQuery = new SQLiteCommand(historyUpdate, connection);
                }
                else
                {
                    nonQuery = new SQLiteCommand(historyInsert, connection);
                    nonQuery.Parameters.AddWithValue("@item", item);
                    
                    nonQuery.Parameters.AddWithValue("@plugin_name", pluginName);
                }
                nonQuery.Parameters.AddWithValue("@snippet", snippet);
                nonQuery.Parameters.AddWithValue("@subitem", subitem);
                nonQuery.Parameters.AddWithValue("@item_info", itemInfo);
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
            if (Properties.Settings.Default.FoldersToIndex == null)
            {
                return;
            }

            ClearDB(indexDbInfo);

            var folderStack = new Stack<FolderToIndex>();
            foreach (var item in Properties.Settings.Default.FoldersToIndex)
            {
                var folder = new FolderToIndex(item);
                if (folder.Extensions.Count > 0)
                {
                    folderStack.Push(folder);
                }
            }

            IEnumerable<string> currentFiles;
            var failedToIndex = new List<string>();

            while (folderStack.Count > 0)
            {
                var currentFolder = folderStack.Pop();

                if (Properties.Settings.Default.FoldersToExclude != null && 
                    Properties.Settings.Default.FoldersToExclude.Contains(currentFolder.Path))
                {
                    continue;
                }

                try
                {
                    currentFiles = Directory.EnumerateFiles(currentFolder.Path, "*.*").Where(file => currentFolder.Extensions.Contains(Path.GetExtension(file)));
                }
                catch (Exception ex)
                {
                    failedToIndex.Add(string.Join(" - ", currentFolder.Path, ex.Message));
                    continue;
                }

                UpdateIndex(currentFiles);

                if (currentFolder.Depth == 0)
                {
                    break;
                }

                foreach (var currentSubFolder in Directory.EnumerateDirectories(currentFolder.Path))
                {
                    folderStack.Push(new FolderToIndex()
                    {
                        Path = currentSubFolder, Depth = currentFolder.Depth - 1,
                        Extensions = currentFolder.Extensions
                    });
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
            foreach (string item in Properties.Settings.Default.FoldersToIndex)
            {
                string result = null;
                var pathEnd = item.IndexOf('|');
                var location = item.Substring(0, pathEnd);
                if (Utils.LocalizePath(location, out result))
                {
                    sc.Add(string.Concat(result, item.Substring(pathEnd)));
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