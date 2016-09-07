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

namespace YalControlPanel
{
    public class YalControlPanel : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public List<string> Activators { get; }

        private Dictionary<string, string> ControlPanelItems { get; }
        private YalControlPanelUC ControlPanelPluginInstance { get; set; }
        string controlPath = string.Concat(Environment.GetEnvironmentVariable("SYSTEMROOT"), @"\system32\control.exe");


        public YalControlPanel()
        {
            Name = "YalControlPanel";
            Version = "1.0";
            Description = "Access Windows control panel items using Yal";

            try
            {
                PluginIcon = Icon.ExtractAssociatedIcon(string.Concat(Directory.GetCurrentDirectory(), @"\plugins\icons\", Name, ".ico"));
            }
            catch
            {
                //MessageBox.Show((Directory.GetCurrentDirectory()));
            }

            ControlPanelItems = new Dictionary<string, string>();

            ControlPanelItems.Add("Accessibility controls", "access.cpl");
            ControlPanelItems.Add("Add/Remove Programs", "appwiz.cpl");
            ControlPanelItems.Add("Display properties", "desk.cpl");
            ControlPanelItems.Add("Add hardware", "hdwwiz.cpl");
            ControlPanelItems.Add("Configure Internet Explorer and Internet properties", "inetcpl.cpl");
            ControlPanelItems.Add("Regional settings", "intl.cpl");
            ControlPanelItems.Add("Game controllers", "joy.cpl");
            ControlPanelItems.Add("Mouse properties and settings", "main.cpl");
            ControlPanelItems.Add("Keyboard properties", "main.cpl,@1 ");
            ControlPanelItems.Add("Sounds and Audio", "mmsys.cpl");
            ControlPanelItems.Add("Network properties", "ncpa.cpl");
            ControlPanelItems.Add("User accounts", "nusrmgr.cpl");
            ControlPanelItems.Add("Power configuration", "powercfg.cpl");
            ControlPanelItems.Add("System properties", "sysdm.cpl");
            ControlPanelItems.Add("Phone and modem options", "telephon.cpl");
            ControlPanelItems.Add("Date and time properties", "timedate.cpl");

            Activators = new List<string>(ControlPanelItems.Keys);
        }

        public void SaveSettings()
        {
            ControlPanelPluginInstance.SaveSettings();
        }

        public UserControl GetUserControl()
        {
            if (ControlPanelPluginInstance == null || ControlPanelPluginInstance.IsDisposed)
            {
                ControlPanelPluginInstance = new YalControlPanelUC();
            }
            return ControlPanelPluginInstance;
        }

        public bool TryParseInput(string input, out string output)
        {
            output = "";
            string ret = GetMatchingActivator(input);
            if (ret == null)
            {
                return false;
            }
            else
            {
                output = ret;
                return true;
            }
        }

        public void HandleExecution(string input)
        {
            string ret = GetMatchingActivator(input);
            if (ret != null)
            {
                Process.Start(controlPath, ControlPanelItems[ret]);
            }
        }

        private string GetMatchingActivator(string input)
        {
            foreach (string activator in Activators)
            {
                if (activator.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                {
                    return activator;
                }
            }
            return null;
        }
    }
}
