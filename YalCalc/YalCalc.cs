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
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public bool FileLikeOutput { get; }

        private List<string> activators;
        private YalCalcUC CalcPluginInstance { get; set; }

        public YalCalc()
        {
            Name = "YalCalc";
            Version = "1.0";
            Description = "Perform simple calculations using Yal";

            FileLikeOutput = false;
            PluginIcon = Utils.GetPluginIcon(Name);
            activators = new List<string>() { "=" };
        }

        public void SaveSettings()
        {
            CalcPluginInstance.SaveSettings();
        }

        public UserControl GetUserControl()
        {
            if (CalcPluginInstance == null || CalcPluginInstance.IsDisposed)
            {
                CalcPluginInstance = new YalCalcUC();
            }
            return CalcPluginInstance;
        }

        public string[] GetResults(string input, bool matchAnywhere, bool fuzzyMatch, out string[] itemInfo)
        {
            itemInfo = null;

            var dt = new DataTable();
            try
            {
                double result = Convert.ToDouble(dt.Compute(input.Substring(1), filter: ""));
                return new string[] { Convert.ToString(Math.Round(result, Properties.Settings.Default.DecimalPlaces)) };
            }
            catch
            {
            }
            return new string[0];
        }

        public void HandleExecution(string input)
        {
            if (Properties.Settings.Default.ReplaceClipboard)
            {
                Clipboard.SetText(input);
            }
        }

        public bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            return input.StartsWith(activators[0]);
        }
    }
}
