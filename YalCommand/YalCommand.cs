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
        public string Name { get; } = "YalCommand";
        public string Version { get; } = "1.0";
        public string Description { get; } = "Yal plugin that allows you to quickly run programs with different parameters";

        public Icon PluginIcon { get; }
        public string HelpText { get; }

        private IEnumerable<string> activators;
        private Regex digitRegex = new Regex(@"\d+");
        internal const string emptyPlaceholder = "~notset~";
        private YalCommandUC CommandPluginInstance { get; set; }
        private Dictionary<string, List<string>> Entries = new Dictionary<string, List<string>>();

        public YalCommand()
        {
            PopulateEntries();
            activators = Entries.Keys;
            PluginIcon = Utils.GetPluginIcon(Name);

            HelpText = $@"This plugin lets you run programs with 1 or more
optional parameters, using shortcuts (commands).
To run a program, type it's assigned shortcut/command
followed by any necessary arguments. If a command takes
no arguments but the user specifies any, they will be
simply ignored.

To add a new command, use the functionality bellow the
list of entries.
If the '{(GetUserControl() as YalCommandUC).lblConfirm.Text}' option
is set to 'True', the user will be prompted with a
message box before the command is ran. The message box
will allow the user to prevent the command from running.

Commands support optional parameters, bellow are the
supported placeholders and their meaning:
!x! - replaced with the value of the argument specified
at runtime at index 'x'.

!x-y! - replaced with the values of the
arguments between indices 'x' and 'y' (inclusive).

!x-n! - replaced with the values of all the
arguments in the input, excluding those with
indices below 'x'.

!n! - replaced with all the available arguments

Question marks ('?') can also be used instead of
exclamation marks ('!'). The parameters that use
question marks are optional. If the command is ran
without an argument at the specified index or range,
the parameter is simply skipped, and no value is
assigned to it.

Examples:
for a command '~nopen' that expects '?n?'
~nopen hello world.txt -> '?n?' is replaced with
'hello world.txt'.

~nopen -> '?n?' is an optional parameter, so the
command runs without any arguments.

for a command '~hhh' that expects '!1!'
~hhh hello world.txt -> '!1!' is replaced with 'hello',
the next argument is ignored.

~hhh -> Error, the command expects an argument, since '!1!'
is mandatory.";
        }

        internal void PopulateEntries()
        {
            if (Entries.Count > 0)
            {
                Entries.Clear();
            }

            foreach (var entry in Properties.Settings.Default.Entries)
            {
                var split = entry.Split('|');

                string path;
                if (!Utils.LocalizePath(split[1], out path))
                {
                    continue;
                }

                // key: command; values: path, parameters, confirm before processing
                Entries.Add(split[0], new List<string>()
                {
                    path, split[2] == emptyPlaceholder ? string.Empty : split[2], split[3]
                });
            }
        }

        public string[] GetResults(string input, out string[] itemInfo)
        {
            itemInfo = null;
            return Entries.Keys.ToArray();
        }

        public UserControl GetUserControl()
        {
            if (CommandPluginInstance == null || CommandPluginInstance.IsDisposed)
            {
                CommandPluginInstance = new YalCommandUC(this, Entries);
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

            if (parameters != string.Empty)
            {
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
                else if (parameters != string.Empty)
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
