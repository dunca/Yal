using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using Utilities;
using PluginInterfaces;

namespace YalBookmark
{
    public class YalBookmark : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public bool FileLikeOutput { get; }

        private const int limit = 5;
        private List<string> activators;
        private YalBookmarkUC BookmarkPluginInstance { get; set; }
        private Dictionary<string, string[]> localQueryCache = new Dictionary<string, string[]>();

        private Dictionary<string, string> browserDbPaths = new Dictionary<string, string>()
        {
            { "Firefox", GetFirefoxDbPath() }
        };
        private const string connectionString = "Data Source={0};Version=3;";
        private Dictionary<string, string> browserSqlQueries = new Dictionary<string, string>()
        {
            {
                "Firefox", @"select bookmarks.TITLE, places.URL from moz_bookmarks as bookmarks, 
                           moz_places as places where bookmarks.fk = places.id and places.REV_HOST is not null
                           and bookmarks.TITLE like @snippet order by bookmarks.TITLE limit @limit"
            }
        };
        private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public YalBookmark()
        {
            Name = "YalBookmark";
            Version = "1.0";
            Description = "Yal plugin that allows you to easily visit your browser bookmarks";

            FileLikeOutput = false;
            PluginIcon = Utils.GetPluginIcon(Name);
            activators = new List<string>();

            browserDbPaths = GetBrowserDbPaths();
        }

        private Dictionary<string, string> GetBrowserDbPaths()
        {
            var paths = new Dictionary<string, string>();

            string firefoxDbPath = GetFirefoxDbPath();
            if (!string.IsNullOrEmpty(firefoxDbPath))
            {
                paths.Add("Firefox", firefoxDbPath);
            }

            return paths;
        }

        private static string GetFirefoxDbPath()
        {
            var firefoxFolder = string.Concat(appDataPath, @"\Mozilla\Firefox");
            if (Directory.Exists(firefoxFolder))
            {
                var profilesFile = string.Concat(firefoxFolder, @"\profiles.ini");
                if (File.Exists(profilesFile))
                {
                    string profilePath = string.Empty;
                    var content = File.ReadAllLines(profilesFile);
                    for (int i = 0; i < content.Length; i++)
                    {
                        if (content[i].StartsWith("Path="))
                        {
                            if (i < content.Length - 1 && content[i + 1].StartsWith("Default") && content[i+1].Split('=')[1] == "1")
                            {
                                // found the default profile
                                profilePath = content[i].Split('=')[1];
                                break;
                            }
                        }
                    }
                    string profileDir = Path.Combine(firefoxFolder, profilePath);
                    string placesSqlite = string.Concat(profileDir, @"\places.sqlite");
                    if (File.Exists(placesSqlite))
                    {
                        return placesSqlite;
                    }
                    return null;
                }
            }
            return null;
        }

        public void SaveSettings()
        {
            BookmarkPluginInstance.SaveSettings();
        }

        public UserControl GetUserControl()
        {
            if (BookmarkPluginInstance == null || BookmarkPluginInstance.IsDisposed)
            {
                BookmarkPluginInstance = new YalBookmarkUC();
            }
            return BookmarkPluginInstance;
        }

        public string[] GetResults(string input, bool matchAnywhere, bool fuzzyMatch, out string[] itemInfo)
        {
            string snippet;
            itemInfo = null;
            localQueryCache.Clear();

            if (fuzzyMatch)
            {
                snippet = string.Concat(input.Select(c => $"{c}%"));
            }
            else
            {
                snippet = $"{input}%";
            }

            if (matchAnywhere)
            {
                snippet = string.Concat("%", snippet);
            }

            foreach (string browser in browserDbPaths.Keys)
            {
                var query = browserSqlQueries[browser];
                using (var connection = new SQLiteConnection(string.Format(connectionString, browserDbPaths[browser])))
                {
                    connection.Open();
                    var command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@snippet", snippet);

                    var response = command.ExecuteReader();

                    while (response.Read())
                    {
                        var bookmarkName = response["TITLE"].ToString();
                        if (bookmarkName.Length > Properties.Settings.Default.Truncate)
                        {
                            bookmarkName = $"{bookmarkName.Substring(0, Properties.Settings.Default.Truncate)}...";
                        }
                        localQueryCache.Add(bookmarkName, new string[] { browser, response["URL"].ToString() });
                    }
                }
            }
            return localQueryCache.Keys.ToArray();
        }

        public void HandleExecution(string input)
        {
            var providingBrowserName = localQueryCache[input][0];
            var url = localQueryCache[input][1];
            Process.Start(url);
        }

        public bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            return browserDbPaths.Count > 0;
        }
    }
}
