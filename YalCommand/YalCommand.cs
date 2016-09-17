using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterfaces;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace YalCommand
{
    public class YalCommand : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public bool FileLikeOutput { get; }

        private List<string> activators;
        private YalCommandUC CommandPluginInstance { get; set; }
        private Dictionary<string, string> Entries;

        public YalCommand()
        {
            Name = "YalCommand";
            Version = "1.0";
            Description = "Yal plugin that allows you to quickly run programs with different parameters";

            try
            {
                PluginIcon = Icon.ExtractAssociatedIcon(string.Concat(Directory.GetCurrentDirectory(), @"\plugins\icons\", Name, ".ico"));
            }
            catch
            {
            }

            FileLikeOutput = false;
        }

        public bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            throw new NotImplementedException();
        }

        public string[] GetResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            throw new NotImplementedException();
        }

        public UserControl GetUserControl()
        {
            if (CommandPluginInstance == null || CommandPluginInstance.IsDisposed)
            {
                CommandPluginInstance = new YalCommandUC();
            }
            return CommandPluginInstance;
        }

        public void HandleExecution(string input)
        {
            throw new NotImplementedException();
        }

        public void SaveSettings()
        {
            CommandPluginInstance.SaveSettings();
        }
    }
}
