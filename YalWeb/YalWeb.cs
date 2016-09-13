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

namespace YalWeb
{
    public class YalWeb : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public List<string> Activators { get; }
        public bool FileLikeOutput { get; }

        private YalWebUC WebPluginInstance { get; set; }
        private Dictionary<string, string> Entries;

        //public string Activator { get; }

        public YalWeb()
        {
            Name = "YalWeb";
            Version = "1.0";
            Description = "Yal plugin that allows you to quickly search the web using your favorite search engine";

            try
            {
                PluginIcon = Icon.ExtractAssociatedIcon(string.Concat(Directory.GetCurrentDirectory(), @"\plugins\icons\", Name, ".ico"));
            }
            catch
            {
                //MessageBox.Show((Directory.GetCurrentDirectory()));
            }
            FileLikeOutput = false;

            Entries = new Dictionary<string, string>();
            foreach (string item in Properties.Settings.Default.Entries)
            {
                var group = item.Split('|');
                Entries.Add(group[0], group[1]);
            }

            Activators = Entries.Keys.ToList();
        }

        public void SaveSettings()
        {
            WebPluginInstance.SaveSettings();
        }

        public UserControl GetUserControl()
        {
            if (WebPluginInstance == null || WebPluginInstance.IsDisposed)
            {
                WebPluginInstance = new YalWebUC();
            }
            return WebPluginInstance;
        }

        public bool TryParseInput(string input, out string[] output, bool matchAnywhere)
        {
            // this plugin is always able to return results (if the user's pc is connected & the search urls work)
            output = new string[] { input };
            return true;
        }

        public void HandleExecution(string input)
        {
            string providerName = input.Substring(0, input.IndexOf(' '));
            string url = Entries[providerName].Replace("%1", input.Substring(input.IndexOf(' ') + 1));
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
