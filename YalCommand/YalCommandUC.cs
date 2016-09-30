using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Utilities;

namespace YalCommand
{
    public partial class YalCommandUC : UserControl
    {
        YalCommand pluginInstance;
        private Dictionary<string, List<string>> Entries;

        internal const string optionalParameterTag = "?";
        internal const string mandatoryParameterTag = "!";

        internal static Regex placeholderRegex = new Regex(@"^(?<TAG>[!\?])(?<ID>[1-9]+[0-9]*|n|[1-9]+[0-9]*-n|[1-9]+[0-9]*-[1-9]+[0-9]*)\k<TAG>$");

        public YalCommandUC(YalCommand pluginInstance, Dictionary<string, List<string>> entires)
        {
            InitializeComponent();

            Entries = entires;
            this.pluginInstance = pluginInstance;

            PopulateListView();
            cbxConfirm.SelectedIndex = 0;
        }

        private void PopulateListView()
        {
            // eg.: ~reboot|%windir%\system32\shutdown.exe|-r|1
            foreach (var entry in Entries)
            {
                listViewEntries.Items.Add(new ListViewItem(new string[] { entry.Key, entry.Value[0], entry.Value[1],
                                                                          Convert.ToBoolean(entry.Value[2]).ToString() }));
            }
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.Entries.Clear();
            foreach (ListViewItem lvi in listViewEntries.Items)
            {
                var command = lvi.SubItems[0].Text;
                var target = lvi.SubItems[1].Text;
                var parameters = lvi.SubItems[2].Text;
                var confirm = lvi.SubItems[3].Text;

                var finalParameters = parameters == "" ? YalCommand.emptyPlaceholder : parameters;
                Properties.Settings.Default.Entries.Add(string.Join("|", command, target, finalParameters, confirm));
            }
            pluginInstance.PopulateEntries();
            Properties.Settings.Default.Save();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var command = txtCommand.Text;
            var target = txtTarget.Text;
            var parameters = txtParameters.Text;
            var confirm = (string)cbxConfirm.SelectedItem;

            string errorMessage = "";

            if (command == "" || target == "")
            {
                errorMessage = "Command and target fields can't be empty";
            }
            else if (!command.StartsWith(pluginInstance.Activator))
            {
                errorMessage = $"Every command name must start with the '{pluginInstance.Activator}' activator";
            }
            else if (listViewEntries.Items.ContainsKey(command))
            {
                errorMessage = $"A command named '{command}' already exists";
            }

            else if (!Utils.PathExists(target))
            {
                errorMessage = "The target should be a file or directory";
            }
            else
            {
                foreach (var item in parameters.Split())
                {
                    if (item.StartsWith(optionalParameterTag) || item.StartsWith(mandatoryParameterTag))
                    {
                        Match match = placeholderRegex.Match(item);
                        string matchValue = match.Groups["ID"].Value;

                        if (!match.Success)
                        {
                            errorMessage = $"Invalid parameter '{item}'";
                            break;
                        }
                        else if (matchValue.Contains('-') && matchValue[matchValue.Length - 1] != 'n')
                        {
                            var split = matchValue.Split('-');
                            if (int.Parse(split[0]) >= int.Parse(split[1]))
                            {
                                errorMessage = $"The first item in a range should be less than the second ({item})";
                                break;
                            }
                        }
                    }
                }
            }

            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            listViewEntries.Items.Add(new ListViewItem(new string[] { command, target, parameters, confirm }));
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var filePathDialog = new OpenFileDialog();
            if ( filePathDialog.ShowDialog() == DialogResult.OK)
            {
                txtTarget.Text = filePathDialog.FileName;
            }
        }

        private void copyCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listViewEntries.FocusedItem.SubItems[0].Text);
        }

        private void copyTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listViewEntries.FocusedItem.SubItems[1].Text);
        }

        private void copyParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listViewEntries.FocusedItem.SubItems[2].Text);
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewEntries.Items.Remove(listViewEntries.FocusedItem);
        }

        private void listViewEntries_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
    }
}