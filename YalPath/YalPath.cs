using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

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

        public string HelpText { get; } = @"The plugin's description is self explanatory.";

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
        }

        public void SaveSettings()
        {
            pluginUserControl.SaveSettings();
        }

        public string[] GetResults(string input, out string[] itemInfo)
        {
            itemInfo = null;
            string[] results = null;

            if (Directory.Exists(input))
            {
                // list the dir's contents if the path ends with the directory separator
                if (input[input.Length - 1] == Path.DirectorySeparatorChar)
                {
                    // it seems that EnumerateFSEntries can't deal with 'junction points' (C:\Documents and Settings -> C:\Users),
                    // so we simply ignore those
                    results = Directory.EnumerateFileSystemEntries(input).Where(path => !Utils.FileIsLink(path)).ToArray();
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
