using System;
using System.Data;
using System.Drawing;
using PluginInterfaces;
using System.Windows.Forms;

using Utilities;

namespace YalCalc
{
    public class YalCalc : IPlugin
    {
        public string Name { get; } = "YalCalc";
        public string Version { get; } = "1.0";
        public string Description { get; } = "Perform simple calculations using Yal";
        public bool RequiresActivator { get; } = false;
        public PluginItemSortingOption SortingOption { get; } = PluginItemSortingOption.ByName;

        public Icon PluginIcon { get; }
        public string HelpText { get; } = @"The plugin's description is self explanatory.";
        public UserControl PluginUserControl
        {
            get
            {
                if (pluginUserControl == null || pluginUserControl.IsDisposed)
                {
                    pluginUserControl = new YalCalcUC();
                }
                return pluginUserControl;
            }
        }

        private YalCalcUC pluginUserControl;

        public YalCalc()
        {
            PluginIcon = Utils.GetPluginIcon(Name);
        }

        public void SaveSettings()
        {
            pluginUserControl.SaveSettings();
        }

        public string[] GetItems(string input, out string[] itemInfo)
        {
            itemInfo = null;
            string[] result = null;

            var dt = new DataTable();
            try
            {
                double output = Convert.ToDouble(dt.Compute(input, filter: ""));
                result = new string[] { string.Concat(input, "=", Convert.ToString(Math.Round(output, Properties.Settings.Default.DecimalPlaces))) };
            }
            catch
            {
            }
            return result;
        }

        public void HandleExecution(string input)
        {
            if (Properties.Settings.Default.ReplaceClipboard)
            {
                Clipboard.SetText(input);
            }
        }
    }
}
