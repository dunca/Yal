using System;
using System.Linq;
using System.Drawing;
using PluginInterfaces;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        private Regex digitRegex;
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
                if (!Utils.LocalizePath(split[1], out path))
                {
                    continue;
                }
                // key: command; values: path, parameters, confirm before processing
                Entries.Add(split[0], new List<string>() { path, split[2], split[3] });
            }

            activators = Entries.Keys;
            digitRegex = new Regex(@"\d+");
        }

        public bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch)
        {
            return activators.Any(activator => input.StartsWith(activator));
        }

        public string[] GetResults(string input, bool matchAnywhere, bool fuzzyMatch, out string[] itemInfo)
        {
            itemInfo = null;
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
            var splitUserInput = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var inputCommand = splitUserInput[0];
            List<string> matchingTarget = Entries[inputCommand];

            var command = matchingTarget[0];
            var parameters = matchingTarget[1];
            var confirmBeforeProcessing = Convert.ToBoolean(matchingTarget[2]);

            var arguments = new List<string>();

            foreach (var item in parameters.Split())
            {
                string currentParameter;
                Match match = YalCommandUC.placeholderRegex.Match(item);

                if (!match.Success)
                {
                    currentParameter = item;
                }
                else
                {
                    var currentIdMatch = match.Groups["ID"];
                    var currentTagMatch = match.Groups["TAG"];

                    bool? result = SkipCurrentParameter(currentIdMatch.Value, currentTagMatch.Value, splitUserInput);

                    if (result == null)
                    {
                        return; // index out of range, simply return
                    }
                    else if (result == true)
                    {
                        continue;  // skip this optional parameter
                    }

                    int number;
                    if (int.TryParse(currentIdMatch.Value, out number))
                    {
                        currentParameter = splitUserInput[number];
                    }
                    else
                    {
                        int start = 1; // we always ignore the first item, which is the input command itself
                        int end = splitUserInput.Length - 1;
                        if (currentIdMatch.Value.Contains('-'))
                        {
                            var splitMatchValue = currentIdMatch.Value.Split('-');
                            start = int.Parse(splitMatchValue[0]);
                            if (splitMatchValue[1] != "n")
                            {
                                end = int.Parse(splitMatchValue[1]);
                            }
                        }
                        currentParameter = string.Join(" ", new ArraySegment<string>(splitUserInput, start, end));
                    }
                }

                arguments.Add(currentParameter);
            }

            try
            {
                if (confirmBeforeProcessing && MessageBox.Show($"Are you sure you want to run {inputCommand}?", this.Name,
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                var proc = new Process();
                proc.StartInfo.FileName = command;

                if (arguments.Count > 0)
                {
                    proc.StartInfo.Arguments = string.Join(" ", arguments);
                }
                else if (parameters != YalCommandUC.emptyPlaceholder)
                {
                    proc.StartInfo.Arguments = parameters;
                }

                proc.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private bool? SkipCurrentParameter(string currentIdValue, string currentTagValue, string[] userInput)
        {
            foreach (Match m in digitRegex.Matches(currentIdValue))
            {
                if (int.Parse(m.Value) > userInput.Length - 1)
                {
                    if (currentTagValue == YalCommandUC.mandatoryParameterTag)
                    {
                        MessageBox.Show("Not enough arguments to run the command", this.Name, MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return null; // the current parameter can't get an argument and it can't be skipped either
                    }
                    return true; // can be skipped since it's optional
                }
            }
            return false; // the user's input has enough arguments
        }

        public void SaveSettings()
        {
            CommandPluginInstance.SaveSettings();
        }
    }
}
