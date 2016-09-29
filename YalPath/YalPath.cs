using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Utilities;
using PluginInterfaces;

namespace YalPath
{
    public class YalPath : IPlugin
    {
        public string Name { get; } = "YalPath";
        public string Description { get; } = "Yal plugin that allows you to view/open the contents of directories by specifying directory paths";
        public string Version { get; } = "1.0";
        public bool RequiresActivator { get; } = false;
        public PluginItemSortingOption SortingOption { get; } = PluginItemSortingOption.ByNameLength;

        public string HelpText { get; }

        public UserControl PluginUserControl
        {
            get
            {
                if (pluginUserControl == null || pluginUserControl.IsDisposed)
                {
                    pluginUserControl = new YalPathUC();
                }
                return pluginUserControl;
            }
        }

        public Icon PluginIcon { get; }
        private YalPathUC pluginUserControl;

        public YalPath()
        {
            PluginIcon = Utils.GetPluginIcon(Name);

            HelpText = $@"With YalPath you can view the content of a directory by
writing it's full path (you can make use of TAB completion), 
including the directory separation character (\) at the end.

Eg.: C:\MyStuff\ will return a list of the files and folders
present in the input directory

If you omit the separation character at the end, you will 
instead get just an entry in the input, and that will be
the content of the search box. This will allow you to easily
open that directory by hitting Enter, if the plugin option
named '{(PluginUserControl as YalPathUC).cbOpenPath.Text}' is checked.
";
        }

        public void SaveSettings()
        {
            pluginUserControl.SaveSettings();
        }

        public string[] GetItems(string input, out string[] itemInfo)
        {
            itemInfo = null;
            string[] results = null;

            //if (Regex.IsMatch(input, "^[a-zA-Z]:$"))
            //{
            //    return GetItems(string.Concat(input, Path.DirectorySeparatorChar), out itemInfo);
            //}

            if (Directory.Exists(input))
            {
                // list the dir's contents if the path ends with the directory separator
                if (input[input.Length - 1] == Path.DirectorySeparatorChar)
                {
                    var resultsList = new List<string>();
                    var itemInfoList = new List<string>();

                    foreach (var entry in Directory.EnumerateFileSystemEntries(input))
                    {
                        // it seems that EnumerateFSEntries can't deal with 'junction points' (C:\Documents and Settings -> C:\Users),
                        // so we simply ignore those
                        if (!Utils.FileIsLink(entry))
                        {
                            itemInfoList.Add(entry);
                            resultsList.Add(Path.GetFileName(entry));
                        }
                    }
                    results = resultsList.ToArray();
                    itemInfo = itemInfoList.ToArray();
                }
                else
                {
                    results = new string[] { input };
                }

            }
            else if (File.Exists(input))
            {
                results = new string[] { input };
            }
            else
            {
                // tries to return an array of items that start with the input path
                // eg.: C:\P -> C:\Program Files, C:\Program Files x86...
                var directory = input.TrimEnd(Path.DirectorySeparatorChar);
                var lastSeparatorIndex = directory.LastIndexOf(Path.DirectorySeparatorChar);
                if (lastSeparatorIndex != -1)
                {
                    directory = directory.Substring(0, lastSeparatorIndex + 1);
                    if (Directory.Exists(directory))
                    {
                        results = Directory.EnumerateFileSystemEntries(directory).Where(
                                            path => path.StartsWith(input, StringComparison.CurrentCultureIgnoreCase) && 
                                            !Utils.FileIsLink(path)).ToArray();
                    }
                }
            }
            return results;
        }

        public void HandleExecution(string input)
        {
            if (Utils.PathExists(input))
            {
                if (Properties.Settings.Default.CopyPath)
                {
                    Clipboard.SetText(input);
                }

                if (Properties.Settings.Default.OpenPath)
                {
                    try
                    {
                        Process.Start(input);
                    }
                    catch (Win32Exception e) when (e.Message == "The operation was canceled by the user")
                    {
                        // Just ignore it. The user probably hit 'No' when prompted by UAC
                    }
                }
            }
            else
            {
                MessageBox.Show($"'{input}' is not a valid file system path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
