using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Utilities;
using PluginInterfaces;

namespace YalPath
{
    public class YalPath : IPlugin
    {
        public string Name { get; } = "YalPath";
        public string Description { get; } = "Yal plugin that allows you to view/open the contents of directories by specifying directory paths";
        public string Version { get; } = "1.0";
        public string Activator { get; } = null;
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

            HelpText = $@"With YalPath you can view the contents of a directory by
writing its full path (you can make use of TAB completion), 
and then including the directory separation character (\)
at the end.

Eg.: C:\MyStuff\ will return a list of the files and folders
present in the input directory

If you omit the separation character at the end, you will 
instead get just an entry in the output, and that will be
the content of the search box. This will allow you to easily
open that directory by hitting Enter, if the plugin option
named '{(PluginUserControl as YalPathUC).cbOpenPath.Text}' is checked.
";
        }

        public void SaveSettings()
        {
            pluginUserControl.SaveSettings();
        }

        public List<PluginItem> GetItems(string userInput)
        {
            //if (Regex.IsMatch(input, "^[a-zA-Z]:$"))
            //{
            //    return GetItems(string.Concat(input, Path.DirectorySeparatorChar), out itemInfo);
            //}

            List<PluginItem> results = new List<PluginItem>();

            if (Directory.Exists(userInput))
            {
                // list the dir's contents if the path ends with the directory separator
                if (userInput[userInput.Length - 1] == Path.DirectorySeparatorChar)
                {
                    // it seems that EnumerateFSEntries can't deal with 'junction points' (C:\Documents and Settings -> C:\Users),
                    // so we simply ignore those
                    var entries = Directory.EnumerateFileSystemEntries(userInput).Where(entry => !Utils.FileIsLink(entry));
                    results.AddRange(entries.Select(entry => CreatePluginItem(Path.GetFileName(entry), entry)));
                }
                else
                {
                    results.Add(CreatePluginItem(userInput));
                }

            }
            else if (File.Exists(userInput))
            {
                results.Add(CreatePluginItem(userInput));
            }
            else
            {
                // tries to return an array of items that start with the input path
                // eg.: C:\P -> C:\Program Files, C:\Program Files x86...
                var directory = userInput.TrimEnd(Path.DirectorySeparatorChar);
                var lastSeparatorIndex = directory.LastIndexOf(Path.DirectorySeparatorChar);
                if (lastSeparatorIndex != -1)
                {
                    directory = directory.Substring(0, lastSeparatorIndex + 1);
                    if (Directory.Exists(directory))
                    {
                        results.AddRange(Directory.EnumerateFileSystemEntries(directory).Where(entry => !Utils.FileIsLink(entry)
                                            && entry.StartsWith(userInput, StringComparison.CurrentCultureIgnoreCase)
                                            ).Select(entry => CreatePluginItem(entry)));
                    }
                }
            }
            return results.Count > 0 ? results : null;
        }

        public void HandleExecution(string input)
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
                catch (Win32Exception ex) when (ex.Message == "The operation was canceled by the user")
                {
                    // Just ignore it. The user probably hit 'No' when prompted by UAC
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private PluginItem CreatePluginItem(string name, string alternativeInfo = null, string subitemName = null, string iconLocation = null)
        {
            return new PluginItem()
            {
                Item = name, Info = alternativeInfo,
                Subitem = subitemName, IconLocation = iconLocation
            };
        }

        public bool CanHandle(string input)
        {
            return Utils.PathExists(input);
        }
    }
}
