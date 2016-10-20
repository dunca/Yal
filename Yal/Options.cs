using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using PluginInterfaces;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Utilities;
using System.Diagnostics;

namespace Yal
{
    public partial class Options : Form
    {
        private Regex extRegex = new Regex(@"^\.\w+$");

        private const byte defaultFolderDepth = 30;
        private const string pluginEnabledTemplate = "cbEnabled";
        private List<CheckBox> pluginEnabledCheckboxes = new List<CheckBox>();
        private const string activationStatisticsTemplate = "Since {0}, {1} has been activated {2} time(s)";

        FolderToIndex currentFolder;

        // this type of list signals its modification which causes our listbox to reread its contents
        private BindingList<string> foldersToExclude = new BindingList<string>();
        private BindingList<FolderToIndex> foldersToIndex = new BindingList<FolderToIndex>();

        private Yal MainWindow { get; }

        public Options(Yal mainWindow)
        {
            InitializeComponent();

            MainWindow = mainWindow;

            UpdateUIVariables();
        }

        private void UpdateUIVariables()
        {
            lblProgramVersion.Text = $"{Application.ProductName} {Application.ProductVersion}";

            if (Properties.Settings.Default.FoldersToIndex != null)
            {
                foreach (var item in Properties.Settings.Default.FoldersToIndex)
                {
                    foldersToIndex.Add(new FolderToIndex(item));
                }
            }
            listBoxLocations.DataSource = foldersToIndex;
            listBoxLocations.ValueMember = listBoxLocations.DisplayMember = "Path";

            if (Properties.Settings.Default.FoldersToExclude != null)
            {
                foreach (var location in Properties.Settings.Default.FoldersToExclude)
                {
                    foldersToExclude.Add(location);
                }
            }
            listBoxExcludedLocations.DataSource = foldersToExclude;

            UpdateIndexingStatus();

            trackBarOpacity.Value = Properties.Settings.Default.Opacity;

            cbVAlignment.Checked = Properties.Settings.Default.VAlignment;

            cbHAlignment.Checked = Properties.Settings.Default.HAlignment;

            cbMoveWithCtrl.Checked = Properties.Settings.Default.MoveWithCtrl;

            cbTopMost.Checked = Properties.Settings.Default.TopMost;

            comboBoxHKMod.DataSource = Enum.GetValues(typeof(FsModifier));
            comboBoxHKMod.SelectedItem = Properties.Settings.Default.FocusModifier;

            var items = (Keys[])Enum.GetValues(typeof(Keys));
            comboBoxHKKey.DataSource = items.Where(x => x == Keys.Space || (Keys.A <= x && x <= Keys.Z)).ToArray();
            comboBoxHKKey.SelectedItem = Properties.Settings.Default.FocusKey;

            cbAutoIndexing.Checked = Properties.Settings.Default.AutoIndexing;

            cbAutostart.Checked = Properties.Settings.Default.Autostart;

            spinMaxItems.Value = Properties.Settings.Default.MaxItems;

            spinMaxVisible.Value = Properties.Settings.Default.MaxVisible;
            
            spinSearchDelay.Value = Properties.Settings.Default.SearchDelay;

            cbExtensionInFileName.Checked = Properties.Settings.Default.ExtensionInFileName;

            spinAutoIndexingInterval.Value = Properties.Settings.Default.AutoIndexingInterval;

            colorDialog1.Color = Properties.Settings.Default.InterfaceColor;

            cbMatchAnywhere.Checked = Properties.Settings.Default.MatchAnywhere;

            spinMaxHistorySize.Value = Properties.Settings.Default.MaxHistorySize;

            cbPluginSelectionsInHistory.Checked = Properties.Settings.Default.PluginSelectionsInHistory;

            spinMaxPluginItems.Value = Properties.Settings.Default.MaxPluginItems;

            cbFuzzyMatching.Checked = Properties.Settings.Default.FuzzyMatching;

            cbPluginItemsFirst.Checked = Properties.Settings.Default.PluginItemsFirst;

            spinMaxNameSize.Value = Properties.Settings.Default.MaxNameSize;

            cbFuzzyMatchingPluginItems.Checked = Properties.Settings.Default.FuzzyMatchingPluginItems;

            cbShowItemIcons.Checked = Properties.Settings.Default.ShowItemIcons;

            cbAutoUpdateCheck.Checked = Properties.Settings.Default.AutoUpdateCheck;

            lblActivationStatistics.Text = string.Format(activationStatisticsTemplate,
                                                         Properties.Settings.Default.DateFirstLaunched,
                                                         Application.ProductName, Properties.Settings.Default.TimesActivated.ToString()
                                                         );

            foreach (IPlugin plugin in MainWindow.pluginInstances)
            {
                var tab = new TabPage() { Text = plugin.Name };
                tabControlPlugins.TabPages.Add(tab);

                // gotta love magic numbers...that just fit
                var panel = new Panel() { Dock = DockStyle.Top, Height = this.Height / 8 };

                var pluginInfo = new Label()
                {
                    Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = plugin.PluginUserControl.BackColor,
                    Text = string.Join(" ", plugin.Name, plugin.Version, string.Concat("- ", plugin.Description))
                };

                var cbPluginEnabled = new CheckBox()
                {
                    Text = "Use this plugin", Checked = !Properties.Settings.Default.DisabledPlugins.Contains(plugin.Name),
                    Dock = DockStyle.Bottom, Name = string.Concat(pluginEnabledTemplate, plugin.Name)
                };

                var helpLabel = new LinkLabel() { Text = "?", AutoSize = true, Dock = DockStyle.Right };
                helpLabel.Click += pluginHelpButton_Click;

                panel.Controls.Add(helpLabel);
                panel.Controls.Add(pluginInfo);
                panel.Controls.Add(cbPluginEnabled);

                pluginEnabledCheckboxes.Add(cbPluginEnabled);

                tab.Controls.AddRange(new Control[] {panel, plugin.PluginUserControl });
                
                // if we don't bring it to front, it will most likely ignore and cover other controls
                plugin.PluginUserControl.BringToFront();
                plugin.PluginUserControl.Dock = DockStyle.Fill;
            }
        }

        private void pluginHelpButton_Click(object sender, EventArgs e)
        {
            var currentTab = tabControlPlugins.SelectedTab;
            IPlugin currentPlugin = MainWindow.pluginInstances.Find(plugin => plugin.Name == currentTab.Text);

            var helpForm = new Form()
            {
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.FixedToolWindow, MaximizeBox = false,
                Text = string.Concat(currentPlugin.Name, " help"), StartPosition = FormStartPosition.CenterScreen
            };

            var helpRichBox = new RichTextBox()
            {
                ReadOnly = true, Text = currentPlugin.HelpText, Dock = DockStyle.Fill,
            };

            helpForm.Controls.Add(helpRichBox);
            helpForm.Show();
        }

        internal void UpdateIndexingStatus()
        {
            btnRebuild.Enabled = true;
            progressBarIndexing.Style = ProgressBarStyle.Blocks;
            lblIndexStatus.Text = $"{FileManager.DbRowCount(FileManager.indexDbInfo)} items indexed at {Properties.Settings.Default.DateLastIndexed}";
        }

        private void btnAddLocation_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (foldersToIndex.Any(item => item.Path == dialog.SelectedPath))
                {
                    MessageBox.Show($"Path '{dialog.SelectedPath}' already exists", "Error", MessageBoxButtons.OK, 
                                    MessageBoxIcon.Error);
                }
                else
                {
                    foldersToIndex.Add(new FolderToIndex()
                    {
                        Path = dialog.SelectedPath, Extensions = new BindingList<string>(), Depth = defaultFolderDepth
                    });

                    if (foldersToIndex.Count == 1)
                    {
                        // it seems that the listbox's SelectedIndexChanged is not triggered when adding the first item
                        // to the data source
                        UpdateSelectedFolder();
                    }
                }
            }
        }

        private void btnRemoveLocation_Click(object sender, EventArgs e)
        {
            if (currentFolder != null)
            {
                foldersToIndex.Remove(currentFolder);
            }
            UpdateSelectedFolder();
        }

        private void spinFolderDepth_ValueChanged(object sender, EventArgs e)
        {
            if (currentFolder != null)
            {
                currentFolder.Depth = (int)spinFolderDepth.Value;
            }
        }

        private void btnAddExt_Click(object sender, EventArgs e)
        {
            if (extRegex.IsMatch(txtExtension.Text) && !currentFolder.Extensions.Contains(txtExtension.Text))
            {
                currentFolder.Extensions.Add(txtExtension.Text);
                txtExtension.Clear();
            }
            else
            {
                MessageBox.Show("Invalid or duplicate file extension(s). Example extension: .exe",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveExt_Click(object sender, EventArgs e)
        {
            currentFolder.Extensions.Remove((string)listBoxExtensions.SelectedItem);
        }

        private void listBoxLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedFolder();
        }

        private void UpdateSelectedFolder()
        {
            if (listBoxLocations.SelectedIndex != -1)
            {
                currentFolder = foldersToIndex[listBoxLocations.SelectedIndex];

                spinFolderDepth.Value = currentFolder.Depth;
                listBoxExtensions.DataSource = currentFolder.Extensions;
            }
            else
            {
                currentFolder = null;
                spinFolderDepth.Value = defaultFolderDepth;
                listBoxExtensions.DataSource = null;
            }

            spinFolderDepth.Enabled = btnAddExt.Enabled = btnRemoveExt.Enabled = currentFolder != null;
        }

        private void btnAddExcludedLocation_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!foldersToExclude.Contains(dialog.SelectedPath))
                {
                    foldersToExclude.Add(dialog.SelectedPath);
                }
            }
        }
        
        private void btnRemoveExcludedLocation_Click(object sender, EventArgs e)
        {
            foldersToExclude.Remove((string)listBoxExcludedLocations.SelectedItem);
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            MainWindow.RebuildIndex();
            btnRebuild.Enabled = false;
            progressBarIndexing.Style = ProgressBarStyle.Marquee;
        }

        private void ManageAppAutoStart()
        {
            string appPath = Application.ExecutablePath;
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true))
            {
                if (Properties.Settings.Default.Autostart)
                {
                    object currentValue = rk.GetValue(MainWindow.Text);
                    if (currentValue == null || (string)currentValue != appPath)
                    {
                        rk.SetValue(MainWindow.Text, appPath);

                        if (Utils.GetOsVersion() == "10")
                        {
                            MessageBox.Show($"On Windows 10 you need to enable {Application.ProductName} from within Task Manager/Startup",
                                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    rk.DeleteValue(MainWindow.Text, throwOnMissingValue: false);
                }
            }
        }

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
        }

        private void btnCancelOpt_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApplyOptions_Click(object sender, EventArgs e)
        {
            var hotkeyMod = (FsModifier)comboBoxHKMod.SelectedItem;
            var hotkeyKey = (Keys)comboBoxHKKey.SelectedItem;
            if (hotkeyMod != Properties.Settings.Default.FocusModifier || hotkeyKey != Properties.Settings.Default.FocusKey)
            {
                Properties.Settings.Default.FocusModifier = hotkeyMod;
                Properties.Settings.Default.FocusKey = hotkeyKey;
                MainWindow.UpdateHotkey();
            }

            if (colorDialog1.Color != Properties.Settings.Default.InterfaceColor)
            {
                Properties.Settings.Default.InterfaceColor = colorDialog1.Color;
                MainWindow.UpdateWindowColor();
            }

            if (trackBarOpacity.Value != Properties.Settings.Default.Opacity)
            {
                Properties.Settings.Default.Opacity = trackBarOpacity.Value;
                MainWindow.UpdateWindowOpacity();
            }
            
            if (cbVAlignment.Checked != Properties.Settings.Default.VAlignment)
            {
                Properties.Settings.Default.VAlignment = cbVAlignment.Checked;
                MainWindow.UpdateVertAlignment();
            }

            if (cbHAlignment.Checked != Properties.Settings.Default.HAlignment)
            {
                Properties.Settings.Default.HAlignment = cbHAlignment.Checked;
                MainWindow.UpdateHorizAlignment();
            }
            
            if (cbTopMost.Checked != Properties.Settings.Default.TopMost)
            {
                Properties.Settings.Default.TopMost = cbTopMost.Checked;
                MainWindow.UpdateWindowTopMost();
            }

            Properties.Settings.Default.MoveWithCtrl = cbMoveWithCtrl.Checked;

            if (cbAutostart.Checked != Properties.Settings.Default.Autostart)
            {
                Properties.Settings.Default.Autostart = cbAutostart.Checked;
                ManageAppAutoStart();
            }

            Properties.Settings.Default.MaxItems = (int)spinMaxItems.Value;
            Properties.Settings.Default.MaxVisible = (int)spinMaxVisible.Value;
            Properties.Settings.Default.SearchDelay = (int)spinSearchDelay.Value;
            Properties.Settings.Default.ExtensionInFileName = cbExtensionInFileName.Checked;

            var autoIndexInterval = (int)spinAutoIndexingInterval.Value;
            if (autoIndexInterval != Properties.Settings.Default.AutoIndexingInterval)
            {
                Properties.Settings.Default.AutoIndexingInterval = autoIndexInterval;
                MainWindow.ManageAutoIndexingTimer();
            }

            if (cbAutoIndexing.Checked != Properties.Settings.Default.AutoIndexing)
            {
                Properties.Settings.Default.AutoIndexing = cbAutoIndexing.Checked;
                MainWindow.ManageAutoIndexingTimer();
            }
            
            Properties.Settings.Default.MatchAnywhere = cbMatchAnywhere.Checked;

            Properties.Settings.Default.FoldersToIndex = foldersToIndex.Select(item => string.Join("|", item.Path, 
                                                                                                   string.Join("," , item.Extensions),
                                                                                                   item.Depth)).ToStringCollection();

            Properties.Settings.Default.FoldersToExclude = foldersToExclude.ToStringCollection();

            Properties.Settings.Default.MaxHistorySize = (int)spinMaxHistorySize.Value;

            Properties.Settings.Default.PluginSelectionsInHistory = cbPluginSelectionsInHistory.Checked;

            Properties.Settings.Default.MaxPluginItems = (int)spinMaxPluginItems.Value;

            Properties.Settings.Default.FuzzyMatching = cbFuzzyMatching.Checked;

            Properties.Settings.Default.PluginItemsFirst = cbPluginItemsFirst.Checked;

            Properties.Settings.Default.MaxNameSize = (int)spinMaxNameSize.Value;

            Properties.Settings.Default.FuzzyMatchingPluginItems = cbFuzzyMatchingPluginItems.Checked;

            if (cbShowItemIcons.Checked != Properties.Settings.Default.ShowItemIcons)
            {
                Properties.Settings.Default.ShowItemIcons = cbShowItemIcons.Checked;
                MainWindow.outputWindow.UpdateIconState();
            }

            if (cbAutoUpdateCheck.Checked != Properties.Settings.Default.AutoUpdateCheck)
            {
                Properties.Settings.Default.AutoUpdateCheck = cbAutoUpdateCheck.Checked;
            }

            Properties.Settings.Default.DisabledPlugins.Clear();
            foreach (var cb in pluginEnabledCheckboxes)
            {
                if (!cb.Checked)
                {
                    Properties.Settings.Default.DisabledPlugins.Add(cb.Name.Replace(pluginEnabledTemplate, ""));
                }
            }

            MainWindow.pluginInstances.ForEach(plugin => plugin.SaveSettings());
        }

        private void spinMaxItems_ValueChanged(object sender, EventArgs e)
        {
            spinMaxVisible.Maximum = spinMaxPluginItems.Maximum = spinMaxItems.Value;
        }

        private void btnRestoreDefaultSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($@"This will restart {Application.ProductName} and restore every setting to its default value (including plugin specific settings)", MainWindow.Name,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                Application.Restart();
            }
        }

        private void listBoxExtensions_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void listBoxExtensions_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop) as string[];

            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(data[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            string errorMessage = null;
            if (lines.Any(line => !extRegex.IsMatch(line)))
            {
                errorMessage = "Drag and drop a text file with the well formatted file extensions (one per line)";
            }
            else if (currentFolder == null)
            {
                errorMessage = "Make sure you select an indexing location first";
            }

            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var extension in lines)
            {
                currentFolder.Extensions.Add(extension);
            }

        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            // Opens the github project page in the default browser;
            Process.Start(Path.Combine(Application.CompanyName, Application.ProductName));
        }

        private void pictureBoxLogo_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void pictureBoxLogo_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }
}
