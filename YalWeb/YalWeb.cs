using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterfaces;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using Utilities;

namespace YalWeb
{
    public class YalWeb : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public bool FileLikeOutput { get; }

        private IEnumerable<string> activators;
        private YalWebUC WebPluginInstance { get; set; }
        private Dictionary<string, string> Entries;

        // when this activator is used, the search is done using the default activator (!ddg/!yt/!a)
        private const string defaultActivator = "!s";

        public YalWeb()
        {
            Name = "YalWeb";
            Version = "1.0";
            Description = "Yal plugin that allows you to quickly search the web using your favorite search engine";

            FileLikeOutput = false;
            PluginIcon = Utils.GetPluginIcon(Name);

            Entries = new Dictionary<string, string>();
            activators = Entries.Keys;
            UpdateEntries();
        }

        public void SaveSettings()
        {
            WebPluginInstance.SaveSettings();
        }

        internal void UpdateEntries()
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

        public string[] GetResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            // this plugin is always able to return results (if the user's pc is connected & the search urls work)
            return new string[] { input };
        }

        public void HandleExecution(string input)
        {
            string providerName = input.Substring(0, input.IndexOf(' '));
            if (providerName == "!s")
            {
                if (Properties.Settings.Default.DefaultEntry != string.Empty)
                {
                    providerName = Properties.Settings.Default.DefaultEntry;
                }
                else
                {
                    MessageBox.Show("The default activator is not set", this.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
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

        public bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            var firstInputItem = input.Split()[0];
            return firstInputItem == defaultActivator || activators.Any(activator => firstInputItem == activator);
        }
    }
}