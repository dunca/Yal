using System;
using System.Data;
using System.Drawing;
using PluginInterfaces;
using System.Windows.Forms;
using System.Collections.Generic;

using Utilities;

namespace YalCalc
{
    public class YalCalc : IPlugin
    {
        public string Name { get; } = "YalCalc";
        public string Version { get; } = "1.0";
        public string Description { get; } = "Perform simple calculations using Yal";
        public string Activator { get; } = null;
        public PluginItemSortingOption SortingOption { get; } = PluginItemSortingOption.ByNameLength;

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

        public List<PluginItem> GetItems(string userInput)
        {
            List<PluginItem> results = null;
            var dt = new DataTable();

            try
            {
                var output = Math.Round(Convert.ToDouble(dt.Compute(userInput, filter: "")), 
                                        Properties.Settings.Default.DecimalPlaces).ToString();
                results = new List<PluginItem>()
                {
                    new PluginItem() { Item = output, Info = string.Concat(userInput, "=", output) }
                };
            }
            catch
            {
                // the above dt.Compute will obviously fail if the input is not a (simple) mathematical expression
            }

            return results;
        }

        public void HandleExecution(string input)
        {
            if (Properties.Settings.Default.ReplaceClipboard)
            {
                Clipboard.SetText(input);
            }
        }

        public bool CanHandle(string input)
        {
            return true;
        }
    }
}
