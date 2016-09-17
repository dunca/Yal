using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using PluginInterfaces;
using Utilities;

namespace Yal
{
    public partial class Options : Form
    {
        Regex extRegex = new Regex(@"^\w+$");

        // this type of list signals it's modification which causes our listbox to reread it's contents
        internal BindingList<string> FoldersToIndex { get; set; }
        internal BindingList<string> FoldersToExclude { get; set; }

        Launcher MainWindow { get; }

        public Options(Launcher mainWindow)
        {
            InitializeComponent();

            MainWindow = mainWindow;

            UpdateUIVariables();
        }

        private void UpdateUIVariables()
        {
            StringCollection locations = Properties.Settings.Default.FoldersToIndex;
            if (locations == null)
            {
                FoldersToIndex = new BindingList<string>();
            }
            else
            {
                FoldersToIndex = new BindingList<string>(locations.Cast<string>().ToList<string>());
            }
            listBoxLocations.DataSource = FoldersToIndex;

            StringCollection excludedLocations = Properties.Settings.Default.FoldersToExclude;
            if (excludedLocations == null)
            {
                FoldersToExclude = new BindingList<string>();
            }
            else
            {
                FoldersToExclude = new BindingList<string>(excludedLocations.Cast<string>().ToList<string>());
            }
            listBoxExcludedLocations.DataSource = FoldersToExclude;

            UpdateIndexingStatus();

            trackBarOpacity.Value = Properties.Settings.Default.Opacity;

            cbVAlign.Checked = Properties.Settings.Default.VAlignment;

            cbHAlign.Checked = Properties.Settings.Default.HAlignment;

            txtExtensions.Text = Properties.Settings.Default.Extensions;

            cbSubdirs.Checked = Properties.Settings.Default.Subdirectories;

            cbCtrlMove.Checked = Properties.Settings.Default.MoveWithCtrl;

            cbTopMost.Checked = Properties.Settings.Default.TopMost;

            comboBoxHKMod.DataSource = Enum.GetValues(typeof(FsModifier));
            comboBoxHKMod.SelectedItem = Properties.Settings.Default.FocusModifier;

            var items = (Keys[])Enum.GetValues(typeof(Keys));
            comboBoxHKKey.DataSource = items.Where(x => x == Keys.Space || (Keys.A <= x && x <= Keys.Z)).ToArray();
            comboBoxHKKey.SelectedItem = Properties.Settings.Default.FocusKey;

            cbAutoIndexing.Checked = Properties.Settings.Default.AutoIndexing;

            checkBoxAutostart.Checked = Properties.Settings.Default.Autostart;

            spinMaxItems.Value = Properties.Settings.Default.MaxItems;

            spinMaxVisible.Value = Properties.Settings.Default.MaxVisible;
            
            spinSearchDelay.Value = Properties.Settings.Default.SearchDelay;

            cbShowExt.Checked = Properties.Settings.Default.ExtensionInFileName;

            spinAutoIndexInterval.Value = Properties.Settings.Default.AutoIndexingInterval;

            colorDialog1.Color = Properties.Settings.Default.InterfaceColor;

            cbMatchAnywhere.Checked = Properties.Settings.Default.MatchAnywhere;

            spinMaxHistorySize.Value = Properties.Settings.Default.MaxHistorySize;

            cbPluginSelectionsInHistory.Checked = Properties.Settings.Default.PluginSelectionsInHistory;

            spinMaxPluginItems.Value = Properties.Settings.Default.MaxPluginItems;

            cbFuzzyMatching.Checked = Properties.Settings.Default.FuzzyMatching;

            foreach (IPlugin plugin in MainWindow.PluginInstances)
            {
                var tab = new TabPage() { Text = plugin.Name };
                tabControlPlugins.TabPages.Add(tab);

                var panel = new Panel() { Dock = DockStyle.Top, Height = this.Height / 12 }; 
                var pluginInfo = new Label() { TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill,
                                               BackColor = plugin.GetUserControl().BackColor };
                panel.Controls.Add(pluginInfo);

                pluginInfo.Text = string.Join(" ", plugin.Name, plugin.Version, string.Concat("- ", plugin.Description));

                tab.Controls.AddRange(new Control[] {panel , plugin.GetUserControl() });
                
                // if we don't bring it to front, it will most likely ignore and cover other controls
                plugin.GetUserControl().BringToFront();
                plugin.GetUserControl().Dock = DockStyle.Fill;
            }
        }

        internal void UpdateIndexingStatus()
        {
            lblIndexStatus.Text = $"{FileManager.DbRowCount(FileManager.indexDbInfo)} items indexed at {Properties.Settings.Default.DateLastIndexed}";
        }

        private void Options_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainWindow.optionsWindow = null;
        }

        private void btnAddLocation_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!FoldersToIndex.Contains(dialog.SelectedPath))
                {
                    FoldersToIndex.Add(dialog.SelectedPath);
                }
            }
        }

        private void btnRemoveLocation_Click(object sender, EventArgs e)
        {
            FoldersToIndex.Remove((string)listBoxLocations.SelectedItem);
        }

        private void btnAddExcludedLocation_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!FoldersToExclude.Contains(dialog.SelectedPath))
                {
                    FoldersToExclude.Add(dialog.SelectedPath);
                }
            }
        }

        private void btnRemoveExcludedLocation_Click(object sender, EventArgs e)
        {
            FoldersToExclude.Remove((string)listBoxExcludedLocations.SelectedItem);
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            FileManager.RebuildIndex();
            UpdateIndexingStatus();
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
                            MessageBox.Show("On Windows 10 you need to enable 'Yal' from within Task Manager/Startup",
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
            if (ValidFileExtensions())
            {
                Properties.Settings.Default.Extensions = txtExtensions.Text;
            }
            else
            {
                txtExtensions.Text = Properties.Settings.Default.Extensions;
                MessageBox.Show("Invalid file extension(s). Correct format: ext or ext1,ext2,extN",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
            
            Properties.Settings.Default.Subdirectories = cbSubdirs.Checked;
            
            if (cbVAlign.Checked != Properties.Settings.Default.VAlignment)
            {
                Properties.Settings.Default.VAlignment = cbVAlign.Checked;
                MainWindow.UpdateVertAlignment();
            }

            if (cbHAlign.Checked != Properties.Settings.Default.HAlignment)
            {
                Properties.Settings.Default.HAlignment = cbHAlign.Checked;
                MainWindow.UpdateHorizAlignment();
            }
            
            if (cbTopMost.Checked != Properties.Settings.Default.TopMost)
            {
                Properties.Settings.Default.TopMost = cbTopMost.Checked;
                MainWindow.UpdateWindowTopMost();
            }

            Properties.Settings.Default.MoveWithCtrl = cbCtrlMove.Checked;

            if (checkBoxAutostart.Checked != Properties.Settings.Default.Autostart)
            {
                Properties.Settings.Default.Autostart = checkBoxAutostart.Checked;
                ManageAppAutoStart();
            }

            Properties.Settings.Default.MaxItems = (int)spinMaxItems.Value;
            Properties.Settings.Default.MaxVisible = (int)spinMaxVisible.Value;
            Properties.Settings.Default.SearchDelay = (int)spinSearchDelay.Value;
            Properties.Settings.Default.ExtensionInFileName = cbShowExt.Checked;

            var autoIndexInterval = (int)spinAutoIndexInterval.Value;
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

            Properties.Settings.Default.FoldersToIndex = FoldersToIndex.ToStringCollection();

            Properties.Settings.Default.FoldersToExclude = FoldersToExclude.ToStringCollection();

            Properties.Settings.Default.MaxHistorySize = (int)spinMaxHistorySize.Value;

            Properties.Settings.Default.PluginSelectionsInHistory = cbPluginSelectionsInHistory.Checked;

            Properties.Settings.Default.MaxPluginItems = (int)spinMaxPluginItems.Value;

            Properties.Settings.Default.FuzzyMatching = cbFuzzyMatching.Checked;

            MainWindow.PluginInstances.ForEach(plugin => plugin.SaveSettings());
        }

        private bool ValidFileExtensions()
        {
            foreach (string ext in txtExtensions.Text.Split(','))
            {
                if (!extRegex.IsMatch(ext))
                {
                    return false;
                }
            }
            return true;
        }

        private void spinMaxItems_ValueChanged(object sender, EventArgs e)
        {
            spinMaxVisible.Maximum = spinMaxPluginItems.Maximum = spinMaxItems.Value;
        }
    }
}
