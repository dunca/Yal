using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.IO;
using PluginInterfaces;

namespace YalCalc
{
    public class YalCalc : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public List<string> Activators { get; }
        public bool FileLikeOutput { get; }

        private YalCalcUC CalcPluginInstance { get; set; }

        //public string Activator { get; }

        public YalCalc()
        {
            Name = "YalCalc";
            Version = "1.0";
            Description = "Perform simple calculations using Yal";

            try
            {
                PluginIcon = Icon.ExtractAssociatedIcon(string.Concat(Directory.GetCurrentDirectory(), @"\plugins\icons\", Name, ".ico"));
            }
            catch
            {
                //MessageBox.Show((Directory.GetCurrentDirectory()));
            }
            FileLikeOutput = false;

            Activators = new List<string>() { "=" };
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

        public bool TryParseInput(string input, out string output)
        {
            var dt = new DataTable();
            try
            {
                double result = Convert.ToDouble(dt.Compute(input.Substring(1), filter: ""));
                output = Convert.ToString(Math.Round(result, Properties.Settings.Default.DecimalPlaces));
                return true;
            }
            catch
            {
            }
            output = "";
            return false;
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
