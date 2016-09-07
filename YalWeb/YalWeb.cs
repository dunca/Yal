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

        private YalWebUC WebPluginInstance { get; set; }

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

        public bool TryParseInput(string input, out string output)
        {
            // this plugin is always able to return results (if the user's pc is connected & the search urls work)
            output = input;
            return true;
        }

        public void HandleExecution(string input)
        {
            string urlPart = string.Empty;
            foreach (string item in Properties.Settings.Default.Entries)
            {
                var group = item.Split('|');
                if (group[0] == Properties.Settings.Default.DefaultEntry)
                {
                    urlPart = group[1];
                    break;
                }
            }
            string url = urlPart.Replace("%1", input);
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
