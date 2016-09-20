using System;
using System.IO;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using Utilities;
using PluginInterfaces;

namespace YalBookmark
{
    struct BrowserInfo
    {
        public string name;
        public string queryString;
        public string databasePath;
        public string executablePath;
        public Func<string, Dictionary<string, string[]>> QueryDatabase;

        public BrowserInfo(string name, string query, string dbPath, string exePath, Func<string, Dictionary<string, string[]>> QueryDatabase)
        {
            this.name = name;
            queryString = query;
            databasePath = dbPath;
            executablePath = exePath;
            this.QueryDatabase = QueryDatabase;
        }
    }

    public class YalBookmark : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public bool FileLikeOutput { get; }

        private const int limit = 5;
        private YalBookmarkUC BookmarkPluginInstance { get; set; }

        private Dictionary<string, BrowserInfo> browsers;
        private Dictionary<string, string[]> localQueryCache = new Dictionary<string, string[]>();
        private static string roamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private static string[] rootRegistryKeys = new string[] { "LOCAL_MACHINE", "CURRENT_USER" };
        private static string appPathsTemplate = @"HKEY_{0}\Software\Microsoft\Windows\CurrentVersion\App Paths\{1}";

        public YalBookmark()
        {
            Name = "YalBookmark";
            Version = "1.0";
            Description = "Yal plugin that allows you to easily visit your browser bookmarks";

            FileLikeOutput = true;
            PluginIcon = Utils.GetPluginIcon(Name);

            browsers = new Dictionary<string, BrowserInfo>()
            {
                { "Firefox",  new BrowserInfo("Firefox", @"select bookmarks.TITLE, places.URL from moz_bookmarks as bookmarks, 
                                                         moz_places as places where bookmarks.fk = places.id and places.REV_HOST is not null
                                                         and bookmarks.TITLE like @snippet order by bookmarks.TITLE limit @limit",
                                              GetFirefoxDbPath(), GetExecutablePath("firefox.exe"), QueryFirefoxDb) },
                { "Chrome", new BrowserInfo("Chrome", "", GetChromeDbPath(), GetExecutablePath("chrome.exe"), QueryChromeDb) }
            };
        }

        private static string GetExecutablePath(string programName)
        {
            foreach (string rootKey in rootRegistryKeys)
            {
                object path = Registry.GetValue(string.Format(appPathsTemplate, rootKey, programName), string.Empty, null);
                if (path != null)
                {
                    return path as string;
                }
            }
            return null;
        }

        private static string GetFirefoxDbPath()
        {
            var firefoxPath = string.Concat(roamingAppData, @"\Mozilla\Firefox");
            var profilesPath = string.Concat(firefoxPath, @"\profiles.ini");
            if (File.Exists(profilesPath))
            {
                string profilePath = string.Empty;
                var content = File.ReadAllLines(profilesPath);

                for (int i = 0; i < content.Length; i++)
                {
                    if (content[i].StartsWith("Path=") && content[i + 1].Split('=')[1] == "1")
                    {
                        // found the default profile's directory
                        profilePath = content[i].Split('=')[1];
                        break;
                    }
                }

                string placesPath = string.Concat(Path.Combine(firefoxPath, profilePath), @"\places.sqlite");
                if (File.Exists(placesPath))
                {
                    return placesPath;
                }
            }
            return null;
        }

        private static string GetChromeDbPath()
        {
            var bookmarksPath = string.Concat(localAppData, @"\Google\Chrome\User Data\Default\Bookmarks");
            return File.Exists(bookmarksPath) ? bookmarksPath : null;
        }

        private Dictionary<string, string[]> QueryFirefoxDb(string snippet)
        {
            var browserInfo = browsers["Firefox"];
            var databasePath = browserInfo.databasePath;

            if (databasePath == null)
            {
                return null;
            }

            var results = new Dictionary<string, string[]>();
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", databasePath)))
            {
                connection.Open();
                var command = new SQLiteCommand(browserInfo.queryString, connection);

                command.Parameters.AddWithValue("@limit", limit);
                command.Parameters.AddWithValue("@snippet", snippet);

                var response = command.ExecuteReader();

                while (response.Read())
                {
                    var title = response["TITLE"].ToString();
                    if (!results.ContainsKey(title))
                    {
                        results.Add(title, new string[] { browserInfo.name, response["URL"].ToString() });
                    }
                }
            }
            return results;
        }

        private Dictionary<string, string[]> QueryChromeDb(string snippet)
        {
            string database = string.Empty;
            var browserInfo = browsers["Chrome"];
            database = File.ReadAllText(browserInfo.databasePath);

            var results = new Dictionary<string, string[]>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic document = serializer.DeserializeObject(database);

            foreach (var bookmark in document["roots"]["bookmark_bar"]["children"])
            {
                if (bookmark["type"] == "url" && bookmark["name"].Contains(snippet.Replace("%", "")) && !results.ContainsKey(bookmark["name"]))
                {
                    results.Add(bookmark["name"], new string[] { browserInfo.name, bookmark["url"] });
                }
            }
            return results;
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

            foreach (BrowserInfo browser in browsers.Values)
            {
                var results = browser.QueryDatabase(snippet);
                if (results != null && results.Count > 0)
                {
                    foreach (var item in results)
                    {
                        if (!localQueryCache.ContainsKey(item.Key))
                        {
                            var bookmarkName = item.Key;
                            if (bookmarkName.Length > Properties.Settings.Default.Truncate)
                            {
                                bookmarkName = $"{bookmarkName.Substring(0, Properties.Settings.Default.Truncate)}...";
                            }
                            localQueryCache.Add(bookmarkName, item.Value);
                        }
                    }
                }
            }
            return localQueryCache.Keys.ToArray();
        }

        public void HandleExecution(string input)
        {
            var providingBrowser = browsers[localQueryCache[input][0]];
            var url = localQueryCache[input][1];
            var proc = new Process();

            if (Properties.Settings.Default.OpenWithProvider && providingBrowser.executablePath != null)
            {
                proc.StartInfo.FileName = providingBrowser.executablePath;
                proc.StartInfo.Arguments = url;
            }
            else
            {
                proc.StartInfo.FileName = url;
            }

            try
            {
                proc.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            return browsers.Any(browser => browser.Value.databasePath != null);
        }
    }
}
