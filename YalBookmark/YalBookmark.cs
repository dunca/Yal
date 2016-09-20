using System;
using System.IO;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

using Utilities;
using PluginInterfaces;

namespace YalBookmark
{
    struct BrowserInfo
    {
        public string name;
        public string queryString;
        public string executableName;
        public Func<string> GetDbPath;
        public Func<string, bool, bool, Dictionary<string, string[]>> QueryDatabase;

        public BrowserInfo(string name, string query, string exeName, Func<string> GetDbPath,
                           Func<string, bool, bool, Dictionary<string, string[]>> QueryDatabase)
        {
            this.name = name;
            queryString = query;
            executableName = exeName;
            this.GetDbPath = GetDbPath;
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
        
        private YalBookmarkUC BookmarkPluginInstance { get; set; }

        private const int limit = 5;
        private Dictionary<string, BrowserInfo> browsers;
        private const string dbConnectionString = "Data Source={0};Version=3;";
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
                                              GetExecutablePath("firefox.exe"), GetFirefoxDbPath, QueryFirefoxDb) },
                { "Chrome", new BrowserInfo("Chrome", "", GetExecutablePath("chrome.exe"), GetChromeDbPath, QueryChromeDb) }
            };
        }

        private static string GetExecutablePath(string programName)
        {
            foreach (string rootKey in rootRegistryKeys)
            {
                var path = (string)Registry.GetValue(string.Format(appPathsTemplate, rootKey, programName), string.Empty, null);
                if (path != null && File.Exists(path))
                {
                    return path;
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
                    if (content[i].StartsWith("Path=") && content[i + 1] == "Default=1")
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

        private Dictionary<string, string[]> QueryFirefoxDb(string snippet, bool matchAnywhere, bool fuzzyMatch)
        {
            var browserInfo = browsers["Firefox"];
            var databasePath = browserInfo.GetDbPath();

            if (databasePath == null)
            {
                return null;
            }

            if (fuzzyMatch)
            {
                snippet = string.Concat(snippet.Select(c => $"{c}%"));
            }
            else
            {
                snippet = $"{snippet}%";
            }

            if (matchAnywhere)
            {
                snippet = string.Concat("%", snippet);
            }

            var results = new Dictionary<string, string[]>();
            using (var connection = new SQLiteConnection(string.Format(dbConnectionString, databasePath)))
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

        private Dictionary<string, string[]> QueryChromeDb(string snippet, bool matchAnywhere, bool fuzzyMatch)
        {
            var browserInfo = browsers["Chrome"];
            string databasePath = browserInfo.GetDbPath();
            
            if (databasePath == null)
            {
                return null;
            }

            var results = new Dictionary<string, string[]>();
            string database = File.ReadAllText(databasePath);

            string regexString = string.Concat("^", snippet);

            if (fuzzyMatch)
            {
                regexString = string.Concat("^", string.Concat(snippet.Select(c => $"{c}.?")));
            }

            if (matchAnywhere)
            {
                regexString = regexString.Substring(1);
            }

            Regex bookmarkRegex = new Regex(regexString, RegexOptions.IgnoreCase);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic parsedBookmarks = serializer.DeserializeObject(database);

            foreach (var bookmark in parsedBookmarks["roots"]["bookmark_bar"]["children"])
            {
                var bookmarkName = bookmark["name"];
                if (bookmark["type"] == "url" && !results.ContainsKey(bookmarkName) && bookmarkRegex.IsMatch(bookmarkName))
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
            itemInfo = null;
            localQueryCache.Clear();

            foreach (BrowserInfo browser in browsers.Values)
            {
                var results = browser.QueryDatabase(input, matchAnywhere, fuzzyMatch);
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

            var providingBrowserPath = GetExecutablePath(providingBrowser.executableName);
            if (Properties.Settings.Default.OpenWithProvider && providingBrowserPath != null)
            {
                proc.StartInfo.FileName = providingBrowserPath;
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
            return browsers.Any(browser => browser.Value.GetDbPath() != null);
        }
    }
}
