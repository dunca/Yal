using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Drawing;

namespace Utilities
{
    public static class Utils
    {
        // used to replace environment variable placeholders inside the app's default indexing paths, with their actual values
        private static Regex envVarRegex = new Regex(@"%([\w\d]+)%");

        public static string GetOsVersion()
        {
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                object value = rk.GetValue("ProductName");
                return Regex.Match(value.ToString(), @"^Windows (Vista|\d+(\.\d+)?) [\w\s]+$").Groups[1].Value;
            }
        }

        public static void OpenFileDirectory(string filePath)
        {
            if (File.Exists(filePath))
            {
                Process.Start(Path.GetDirectoryName(filePath));
            }
        }

        /// <summary>
        /// This method replaces environment variable placeholders with the actual variables
        /// </summary>
        /// <param name="oldPath">Eg. C:\some\cute\dir\%username%\nice</param>
        /// <param name="newPath">Eg. C:\some\cute\dir\Johnny\nice</param>
        /// <returns>a boolean that indicates if the path actually exists</returns>
        public static bool PersonalizePath(string oldPath, out string newPath)
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

            if (PathExists(newPath))
            {
                return true;
            }

            newPath = null;
            return false;
        }

        public static bool IsUrl(string input)
        {
            return Uri.IsWellFormedUriString(input, UriKind.Absolute);
        }

        public static bool IsUrlOrPathExists(string input)
        {
            return IsUrl(input) || PathExists(input);
        }

        public static bool PathExists(string input)
        {
            return Directory.Exists(input) || File.Exists(input);
        }

        public static Icon GetPluginIcon(string iconName)
        {
            var iconPath = $@"plugins\icons\{iconName}.ico";
            if (File.Exists(iconPath))
            {
                return Icon.ExtractAssociatedIcon(iconPath);
            }
            return null;
        }
    }
}
