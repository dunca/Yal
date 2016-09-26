﻿using System;
using System.IO;
using System.Drawing;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class Utils
    {
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
        public static bool LocalizePath(string oldPath, out string newPath)
        {
            newPath = Environment.ExpandEnvironmentVariables(oldPath);

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

        public static bool FileIsLink(string path)
        {
            // http://stackoverflow.com/a/21558051
            return new FileInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        public static bool DirIsLink(string path)
        {
            // http://stackoverflow.com/a/21558051
            return new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
    }
}
