using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Utilities;
using PluginInterfaces;

namespace YalWeb
{
    public class YalWeb : IPlugin
    {
        public string Name { get; } = "YalWeb";
        public string Version { get; } = "1.0";
        public string Description { get; } = "Yal plugin that allows you to quickly search the web using your favorite search engine";
        
        public Icon PluginIcon { get; }
        public string HelpText { get; } = @"To use this plugin, type an activator's
name, followed by the keyworld you'd like to search for.
For example: -> !w rats <- will (by default) search 
Wikipedia for the 'rats' keyword.

You can add new shortcuts using the input boxes
located bellow the current list of entries.
The '%1' character group can be optionally
used in newly added shortcut URLs as a
placeholder for the keywords specified 
at runtime after the shortcut's name.

To remove an entry, right click on it, and
pick the self explanatory menu item.";

        private YalWebUC WebPluginInstance { get; set; }
        private Dictionary<string, string> Entries = new Dictionary<string, string>();

        public YalWeb()
        {
            PopulateEntries();
            PluginIcon = Utils.GetPluginIcon(Name);
        }

        public void SaveSettings()
        {
            WebPluginInstance.SaveSettings();
        }

        internal void PopulateEntries()
        {
            Entries.Clear();
            foreach (string item in Properties.Settings.Default.Entries)
            {
                var group = item.Split('|');
                Entries.Add(group[0], group[1]);
            }
        }

        public UserControl GetUserControl()
        {
            if (WebPluginInstance == null || WebPluginInstance.IsDisposed)
            {
                WebPluginInstance = new YalWebUC(this);
            }
            return WebPluginInstance;
        }

        public string[] GetResults(string input, out string[] itemInfo)
        {
            itemInfo = null;
            
            foreach (var activator in Entries.Keys)
            {
                input = input.Replace(activator, "");
            }

            input = input.TrimStart();
            return Entries.Keys.Select(activator => string.Join(" ", activator, input)).ToArray();
        }

        public void HandleExecution(string input)
        {
            string providerName = input.Substring(0, input.IndexOf(' '));
            string url = Entries[providerName].Replace("%1", Uri.EscapeDataString(input.Substring(input.IndexOf(' ') + 1)));
            try
            {
                Process.Start(url);
            }
            catch (Win32Exception e)  // as per https://support.microsoft.com/en-us/kb/305703 this usually occurs when there's no browser installed
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}