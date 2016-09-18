using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterfaces;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Utilities;

namespace YalCommand
{
    public class YalCommand : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public bool FileLikeOutput { get; }

        private Regex placeholderRegex;
        private IEnumerable<string> activators;
        private YalCommandUC CommandPluginInstance { get; set; }
        private Dictionary<string, List<string>> Entries;

        public YalCommand()
        {
            Name = "YalCommand";
            Version = "1.0";
            Description = "Yal plugin that allows you to quickly run programs with different parameters";

            FileLikeOutput = false;
            PluginIcon = Utils.GetPluginIcon(Name);

            Entries = new Dictionary<string, List<string>>();
            foreach (var entry in Properties.Settings.Default.Entries)
            {
                var split = entry.Split('|');
                string path;
                if (!Utils.PersonalizePath(split[1], out path))
                {
                    continue;
                }
                // key: command; values: path, parameters, confirm before processing
                Entries.Add(split[0], new List<string>() { path, split[2], split[3] });
            }

            activators = Entries.Keys;
            placeholderRegex = new Regex(@"([!\?])([1-9])\1");
        }

        public bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            return activators.Any(activator => input.StartsWith(activator));
        }

        public string[] GetResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            return new string[] { input };
        }

        public UserControl GetUserControl()
        {
            if (CommandPluginInstance == null || CommandPluginInstance.IsDisposed)
            {
                CommandPluginInstance = new YalCommandUC(Entries);
            }
            return CommandPluginInstance;
        }

        public void HandleExecution(string input)
        {
            // there could be parameters in the input
            var split = input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var commandName = split[0];
            var suppliedArgCount = split.Length - 1;
            List<string> matchingTarget = Entries[commandName];
            var command = matchingTarget[0];
            var parameters = matchingTarget[1];
            var confirmBeforeProcessing = Convert.ToBoolean(matchingTarget[2]);

            var matches = placeholderRegex.Matches(parameters);

            for (int i = 0; i < matches.Count; i++)
            {
                if (i == suppliedArgCount)
                {
                    // if there are not enough args in the input and the current parameter is mandatory
                    if (matches[i].Value[0] == '!')
                    {
                        MessageBox.Show($"Not enough arguments to run {commandName}", this.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        break; // for optional parameters that didn't have a supplied argument
                    }
                }
                parameters = parameters.Replace(matches[i].Value, split[i + 1]);
            }

            try
            {
                if (confirmBeforeProcessing && MessageBox.Show($"Are you sure you want to run {commandName}?", this.Name,
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                Process.Start(command, parameters);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void SaveSettings()
        {
            CommandPluginInstance.SaveSettings();
        }
    }
}
