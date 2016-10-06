using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

using Utilities;
using PluginInterfaces;

namespace Yal
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms646309(v=vs.85).aspx
    internal enum FsModifier
    {
        ALT = 0x0001,
        CTRL = 0x0002,
        SHIFT = 0x0004,
        WIN = 0x0008
    }

    public partial class Yal : Form
    {
        private const int HOTKEY_REG_ID = 1;
        private const int WM_HOTKEY = 0x312;  // message code that occurs when hotkeys are detected

        private Timer timerSearchDelay;
        private Timer timerTrimHistory;

        private bool lmbIsDown;
        private Point lastPointerLocation;

        private Options optionsWindow;
        internal OutputWindow outputWindow;
        internal List<IPlugin> pluginInstances;
        private Dictionary<string, int> iconIndexMap = new Dictionary<string, int>();

        private bool shouldAutocomplete;
        private const string attachTemplate = "attach database '{0}' as {1}";
        private const string pluginTableSchema = "create table if not exists PLUGIN_ITEM (ITEM text, SUBITEM text, ITEM_INFO text, PLUGIN_NAME text, REQUIRES_ACTIVATOR numeric, SORT_BY_NAME numeric, ICON_PATH text)";
        private const string pluginInsertString = "insert into PLUGIN_ITEM values (@item, @subitem, @item_info, @plugin_name, @requires_activator, @sort_by_name, @icon_path)";
        private const string itemQueryString = @"
select ITEM, SUBITEM, ITEM_INFO, ICON_PATH, PLUGIN_NAME, MAX(HITS) as MAX_HITS, IS_HISTORY_ITEM from 
(
	select ITEM, SUBITEM, ITEM_INFO, HITS, 1 as SORT_BY_NAME, ROWID, PLUGIN_NAME, ICON_PATH, 1 as IS_HISTORY_ITEM from HISTORY_CATALOG where SNIPPET like @snippet
	union
	select NAME as ITEM, FULLPATH as SUBITEM, '' as ITEM_INFO, @file_priority as HITS, 1 as SORT_BY_NAME, ROWID, '' as PLUGIN_NAME, '' as ICON_PATH, 0 as IS_HISTORY_ITEM from INDEX_CATALOG where NAME like @pattern
	union
	select ITEM, SUBITEM, ITEM_INFO, -1 as HITS, SORT_BY_NAME, ROWID, PLUGIN_NAME, ICON_PATH, 0 as IS_HISTORY_ITEM from PLUGIN_ITEM where (REQUIRES_ACTIVATOR = 0 and ITEM_INFO like @plugin_pattern) OR (REQUIRES_ACTIVATOR = 1 and ITEM_INFO like @activator_plugin_pattern)
) group by ITEM, SUBITEM, ITEM_INFO, ICON_PATH, PLUGIN_NAME order by MAX_HITS desc, case SORT_BY_NAME when 1 then (case PLUGIN_NAME when '' then ITEM else length(ITEM) end) else -ROWID end limit @limit";

        private SQLiteConnection pluginItemDb = new SQLiteConnection("FullUri=file::memory:?cache=shared;Version=3;");

        public Yal()
        {
            InitializeComponent();
            shouldAutocomplete = StandardSearch();

            Directory.SetCurrentDirectory(Application.StartupPath);
            outputWindow = new OutputWindow(this);
            ManageAutoIndexingTimer();
            UpdateWindowLocation();
            UpdateWindowLooks();
                        
            timerSearchDelay = new Timer();
            timerSearchDelay.Tick += SearchDelayTimer_Tick;

            timerTrimHistory = new Timer();
            timerTrimHistory.Interval = 60 * 60 * 1000;  // hourly
            timerTrimHistory.Tick += TrimHistoryTimer_Tick;

            Properties.Settings.Default.FoldersToIndex = FileManager.ProcessRawPaths();

            if (Properties.Settings.Default.DisabledPlugins == null)
            {
                Properties.Settings.Default.DisabledPlugins = new StringCollection();
            }
            
            if (Properties.Settings.Default.DateFirstLaunched == "")
            {
                Properties.Settings.Default.DateFirstLaunched = DateTime.Now.ToShortDateString();
            }

            pluginInstances = PluginManager.InstantiatePlugins();

            pluginItemDb.Open();
            (new SQLiteCommand(string.Format(pluginTableSchema, "PLUGIN_ITEM"), pluginItemDb)).ExecuteNonQuery();
        }

        internal void ShowOptionsWindow()
        {
            if (optionsWindow == null || optionsWindow.IsDisposed)
            {
                optionsWindow = new Options(this);
            }
            optionsWindow.Show();
        }

        internal async void RebuildIndex()
        {
            await FileManager.RebuildIndexAsync();
            optionsWindow?.UpdateIndexingStatus();
        }

        private void TrimHistoryTimer_Tick(object sender, EventArgs e)
        {
            FileManager.TrimHistory();
        }

        internal void ManageAutoIndexingTimer()
        {
            if (Properties.Settings.Default.AutoIndexing)
            {
                timerAutoIndexing.Stop();
                timerAutoIndexing.Interval = Properties.Settings.Default.AutoIndexingInterval * 1000 * 60;
                timerAutoIndexing.Tick -= TimerAutoIndexing_Tick;
                timerAutoIndexing.Tick += TimerAutoIndexing_Tick;
                timerAutoIndexing.Start();
            }
            else
            {
                timerAutoIndexing.Stop();
            }
        }

        private void TimerAutoIndexing_Tick(object sender, EventArgs e)
        {
            RebuildIndex();
        }

        internal void UpdateHotkey()
        {
            // re-registering picks up updated properties set by the user throught the Options form
            UnregisterHotkey();
            RegisterHotkey();
        }

        private void UpdateWindowLocation()
        {
            this.Location = Properties.Settings.Default.Location;

            UpdateVertAlignment();
            UpdateHorizAlignment();
            UpdateWindowTopMost();
        }

        internal void UpdateVertAlignment()
        {
            if (Properties.Settings.Default.VAlignment)
            {
                int screenHeight = Screen.FromControl(this).Bounds.Height;
                this.Location = new Point(this.Location.X, screenHeight / 2 - (this.Height / 2));
            }
        }

        internal void UpdateHorizAlignment()
        {
            if (Properties.Settings.Default.HAlignment)
            {
                int screenWidth = Screen.FromControl(this).Bounds.Width;
                this.Location = new Point(screenWidth / 2 - (this.Width / 2), this.Location.Y);
            }
        }

        internal void UpdateWindowTopMost()
        {
            // set the window to be always on top if necessary
            this.TopMost = outputWindow.TopMost = Properties.Settings.Default.TopMost;
        }

        private void UpdateWindowLooks()
        {
            UpdateWindowColor();
            UpdateWindowOpacity();
        }

        internal void UpdateWindowColor()
        {
            this.BackColor = Properties.Settings.Default.InterfaceColor;
        }

        internal void UpdateWindowOpacity()
        {
            this.Opacity = outputWindow.Opacity = Properties.Settings.Default.Opacity / 10.0;
        }

        // http://www.fluxbytes.com/csharp/how-to-register-a-global-hotkey-for-your-application-in-c/
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr windowHandler, int regId);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr windowHandler, int regId, uint modifiers, uint virtualKey);

        private void RegisterHotkey()
        {
            if (!RegisterHotKey(this.Handle, HOTKEY_REG_ID, (uint)Properties.Settings.Default.FocusModifier,
                                (uint)Properties.Settings.Default.FocusKey))
            {
                var combo = $"{Properties.Settings.Default.FocusModifier}-{Properties.Settings.Default.FocusKey}";
                MessageBox.Show($"Key combination {combo} is already in use. Please choose another", this.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowOptionsWindow();
            }
        }

        private void UnregisterHotkey()
        {
            UnregisterHotKey(this.Handle, HOTKEY_REG_ID);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if (this.txtSearch.Focused)
                {
                    this.Hide();
                }
                else
                {
                    ActivateYal();
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab && TabComplete())
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        internal bool TabComplete()
        {
            if (outputWindow.listViewOutput.SelectedIndices.Count > 0)
            {
                txtSearch.Text = outputWindow.listViewOutput.SelectedItems[0].SubItems[2].Text;
                txtSearch.SelectionStart = txtSearch.Text.Length;
                return true;
            }
            return false;
        }

        private void ActivateYal()
        {
            this.Show();
            this.Activate();

            if (optionsWindow == null)
            {
                txtSearch.Focus();
            }
            else
            {
                optionsWindow.Focus();
            }

            Properties.Settings.Default.TimesActivated += 1;
        }

        private void Yal_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotkey();
            Properties.Settings.Default.Location = Location;
            Properties.Settings.Default.Save();
        }

        private void Yal_Move(object sender, EventArgs e)
        {
            outputWindow.Location = new Point(this.Location.X, this.Location.Y + this.Height);
        }

        private void Yal_Shown(object sender, EventArgs e)
        {
            ActivateYal();
        }

        private void Yal_MouseMove(object sender, MouseEventArgs e)
        {
            if ((lmbIsDown && !Properties.Settings.Default.MoveWithCtrl) ||
                (lmbIsDown && Properties.Settings.Default.MoveWithCtrl &&
                (Control.ModifierKeys == Keys.Control)))
            {
                this.Location = new Point(this.Location.X + (e.X - lastPointerLocation.X),
                                          this.Location.Y + (e.Y - lastPointerLocation.Y));
            }
        }

        private void Yal_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lmbIsDown = true;
                lastPointerLocation = e.Location;
                this.Cursor = Cursors.SizeAll;  // the little crosshair like cursor
            }
        }

        private void Yal_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lmbIsDown = false;
                this.Cursor = Cursors.Default;  // we only want the crosshair cursor when moving the window
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOptionsWindow();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (outputWindow.listViewOutput.SelectedItems.Count != 0 && e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
            {
                outputWindow.BuildContextMenu();
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (!outputWindow.Visible && outputWindow.listViewOutput.Items.Count > 0)
                {
                    outputWindow.Show();
                }
                outputWindow.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                StartSelectedItem(e.Modifiers == (Keys.Shift | Keys.Control), !(e.Modifiers == Keys.Shift));
            }
            else if (e.KeyCode == Keys.Escape)
            {
                outputWindow.Hide();
            }
            else if (e.KeyCode == Keys.Back && StandardSearch())
            {
                if (shouldAutocomplete)
                {
                    shouldAutocomplete = false;
                }
            }
            else if (!shouldAutocomplete && StandardSearch())
            {
                shouldAutocomplete = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                // ignore the up arrow key
                e.Handled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "" && txtSearch.Text == txtSearch.SelectedText)
            {
                // Selecting all the text seems to trigger TextChanged. We don't really want to re-query everyting in this case
                return;
            }

            timerSearchDelay.Stop();
            timerSearchDelay.Interval = Properties.Settings.Default.SearchDelay;
            timerSearchDelay.Start();
        }

        private void SearchDelayTimer_Tick(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            timerSearchDelay.Stop();

            iconIndexMap.Clear();
            outputWindow.imageList1.Images.Clear();
            outputWindow.listViewOutput.Items.Clear();

            var userInput = txtSearch.Text;

            if (userInput != "")
            {
                foreach (var plugin in pluginInstances)
                {
                    if (PluginManager.PluginIsDisabled(plugin) || (plugin.Activator != null && !userInput.StartsWith(plugin.Activator)))
                    {
                        continue;
                    }

                    List<PluginItem> pluginItems = plugin.GetItems(userInput);

                    if (pluginItems != null && pluginItems.Count > 0)
                    {
                        foreach (var pluginItem in pluginItems)
                        {
                            var command = new SQLiteCommand(pluginInsertString, pluginItemDb);
                            
                            command.Parameters.AddWithValue("@sort_by_name", plugin.SortingOption == PluginItemSortingOption.ByNameLength ? 1 : 0);
                            command.Parameters.AddWithValue("@requires_activator", plugin.Activator != null ? 1 : 0);
                            command.Parameters.AddWithValue("@item_info", pluginItem.Info ?? pluginItem.Item);
                            command.Parameters.AddWithValue("@subitem", pluginItem.Subitem ?? plugin.Name);
                            command.Parameters.AddWithValue("@icon_path", pluginItem.IconLocation ?? "");
                            command.Parameters.AddWithValue("@plugin_name", plugin.Name);
                            command.Parameters.AddWithValue("@item", pluginItem.Item);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                using (var connection = FileManager.GetDbConnection(FileManager.historyDbInfo))
                {
                    new SQLiteCommand(string.Format(attachTemplate, "file::memory:?cache=shared", "PLUGIN_ITEM"), connection).ExecuteNonQuery();
                    new SQLiteCommand(string.Format(attachTemplate, FileManager.indexDbInfo.fileName, "INDEX_DB"), connection).ExecuteNonQuery();

                    string pattern = ConstructSearchPattern(userInput, Properties.Settings.Default.FuzzyMatching);
                    string pluginPattern = ConstructSearchPattern(userInput, Properties.Settings.Default.FuzzyMatchingPluginItems);
                    string activatorPluginPattern = ConstructSearchPattern(userInput, Properties.Settings.Default.FuzzyMatchingPluginItems, true);

                    var spaceIndex = activatorPluginPattern.IndexOf(" ");
                    if (spaceIndex != -1 && Properties.Settings.Default.MatchAnywhere)
                    {
                        activatorPluginPattern = activatorPluginPattern.Insert(spaceIndex + 1, "%");
                    }

                    var command = new SQLiteCommand(itemQueryString, connection);

                    command.Parameters.AddWithValue("@file_priority", Properties.Settings.Default.PluginItemsFirst ? -2 : 0);
                    command.Parameters.AddWithValue("@activator_plugin_pattern", activatorPluginPattern);
                    command.Parameters.AddWithValue("@limit", Properties.Settings.Default.MaxItems);
                    command.Parameters.AddWithValue("@snippet", string.Concat(userInput, "%"));
                    command.Parameters.AddWithValue("@plugin_pattern", pluginPattern);
                    command.Parameters.AddWithValue("@pattern", pattern);
                    var reader = command.ExecuteReader();

                    outputWindow.listViewOutput.BeginUpdate();

                    int iconIndex = 0;
                    while (reader.Read())
                    {
                        var item = reader["ITEM"].ToString();
                        var subitem = reader["SUBITEM"].ToString();
                        var pluginName = reader["PLUGIN_NAME"].ToString();

                        ListViewItem lvi = null;
                        if (pluginName != "")
                        {
                            var itemInfo = reader["ITEM_INFO"].ToString();
                            IPlugin pluginInstance = pluginInstances.Find(plugin => plugin.Name == pluginName);
                            if (PluginManager.PluginIsDisabled(pluginInstance) || (Convert.ToInt16(reader["IS_HISTORY_ITEM"]) == 1 && !pluginInstance.CanHandle(itemInfo)))
                            { 
                              // The current item comes for the history database. We either don't want to process it because the plugin
                              // that generated it is disabled, or because the plugin cannot handle it. Eg.: YalProcessKiller can't hanle processes that
                              // were terminated a long time ago
                                continue;
                            }

                            var iconPath = reader["ICON_PATH"].ToString();
                            lvi = new ListViewItem(new string[] { item, subitem, itemInfo != "" ? itemInfo : item, pluginInstance.Name, iconPath });

                            if (Properties.Settings.Default.ShowItemIcons && pluginInstance.PluginIcon != null)
                            {
                                Icon itemIcon = null;

                                if (iconPath != "")
                                {
                                    if (iconIndexMap.ContainsKey(iconPath))
                                    {
                                        lvi.ImageIndex = iconIndexMap[iconPath];
                                    }
                                    else
                                    {
                                        itemIcon = Utils.GetFileIcon(iconPath);
                                    }
                                }

                                if (lvi.ImageIndex == -1)
                                {
                                    if (itemIcon == null)
                                    {
                                        UsePluginIcon(pluginInstance, lvi, ref iconIndex);
                                    }
                                    else
                                    {
                                        iconIndexMap.Add(iconPath, iconIndex);
                                        UpdateImageList(itemIcon, lvi, ref iconIndex);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //  most likely a file
                            if (!Properties.Settings.Default.ExtensionInFileName)
                            {
                                item = Path.GetFileNameWithoutExtension(subitem);
                            }

                            lvi = new ListViewItem(new string[] { item, subitem, item }) { ToolTipText = subitem };

                            Icon icon;
                            if (Properties.Settings.Default.ShowItemIcons && FileManager.GetFileIcon(subitem, out icon))
                            {
                                UpdateImageList(icon, lvi, ref iconIndex);
                            }
                        }
                        lvi.SubItems[0].Text = TrimStringIfNeeded(item);
                        outputWindow.listViewOutput.Items.Add(lvi);
                    }

                    outputWindow.listViewOutput.EndUpdate();
                    new SQLiteCommand("delete from PLUGIN_ITEM", pluginItemDb).ExecuteNonQuery();
                }

                if (outputWindow.listViewOutput.Items.Count > 0)
                {
                    outputWindow.Show(); // Show() it first, so that the listview's ClientSize property gets updated

                    outputWindow.listViewOutput.Items[0].Selected = true;
                    outputWindow.ResizeToFitContent();
                    txtSearch.Focus();  // Show()-ing a window focuses on it by default. We don't want that in this case;

                    if (shouldAutocomplete && StandardSearch() && (outputWindow.listViewOutput.SelectedItems[0].SubItems[0].Text == 
                        outputWindow.listViewOutput.SelectedItems[0].SubItems[2].Text))
                    {
                        txtSearch.TextChanged -= txtSearch_TextChanged;
                        txtSearch.Text = outputWindow.listViewOutput.SelectedItems[0].SubItems[0].Text;
                        txtSearch.SelectionStart = userInput.Length;
                        txtSearch.SelectionLength = txtSearch.Text.Length - userInput.Length;
                        txtSearch.TextChanged += txtSearch_TextChanged;
                    }
                }
                else
                {
                    outputWindow.Hide();
                }
            }
            else
            {
                outputWindow.Hide();
            }          
        }

        private void UpdateImageList(Icon icon, ListViewItem lvi, ref int iconIndex)
        {
            outputWindow.imageList1.Images.Add(icon);
            lvi.ImageIndex = iconIndex;
            iconIndex++;
        }

        private void UsePluginIcon(IPlugin pluginInstance, ListViewItem lvi, ref int iconIndex)
        {
            if (iconIndexMap.ContainsKey(pluginInstance.Name))
            {
                lvi.ImageIndex = iconIndexMap[pluginInstance.Name];
            }
            else
            {
                iconIndexMap.Add(pluginInstance.Name, iconIndex);
                UpdateImageList(pluginInstance.PluginIcon, lvi, ref iconIndex);
            }
        }

        private string ConstructSearchPattern(string input, bool fuzzyMatch, bool ignoreMatchAnywhere = false)
        {
            var pattern =  fuzzyMatch ? string.Concat(input.Select(c => string.Concat(c, "%"))) :
                                        string.Concat(input, "%");
            if (!ignoreMatchAnywhere)
            {
                pattern = string.Concat(Properties.Settings.Default.MatchAnywhere ? "%" : "", pattern);
            }
            return pattern;
        }

        private string TrimStringIfNeeded(string str)
        {
            if (str.Length > Properties.Settings.Default.MaxNameSize)
            {
                str = $"{str.Substring(0, Properties.Settings.Default.MaxNameSize)}...";
            }
            return str;
        }

        internal void StartSelectedItem(bool elevatedRights = false, bool keepInHistory = true)
        {
            if (outputWindow.listViewOutput.SelectedItems.Count == 0)
            {
                return;
            }

            if (!Properties.Settings.Default.TopMost)
            {
                this.Hide();
                outputWindow.Hide();
            }

            var lvi = outputWindow.listViewOutput.SelectedItems[0];

            // the 3rd item in the row is the identifier of the item. The first item
            // in the row is also derived from the identifier. Its length is trimmed based on the user's preference
            string item = lvi.SubItems[2].Text;

            if (lvi.SubItems.Count == 5)
            {
                var plugin = pluginInstances.Find(p => p.Name == lvi.SubItems[3].Text);
                plugin.HandleExecution(item);

                if (Properties.Settings.Default.PluginSelectionsInHistory)
                {
                    FileManager.UpdateHistory(txtSearch.Text, lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text, plugin.Name, lvi.SubItems[4].Text);
                }
                return;
            }

            // the 2nd item in each row. Usually a full file path
            string subitem = outputWindow.listViewOutput.SelectedItems[0].SubItems[1].Text;

            var startInfo = new ProcessStartInfo(subitem);

            if (elevatedRights)
            {
                // http://stackoverflow.com/questions/133379/elevating-process-privilege-programatically
                startInfo.Verb = "runas";
                startInfo.UseShellExecute = true;
            }

            if (Process.Start(startInfo) == null)
            {
                if (MessageBox.Show("Couldn't start process. Remove entry from the database?",
                                    "Issue", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                    == DialogResult.Yes)
                {
                    FileManager.RemoveFromDb(subitem, FileManager.indexDbInfo);
                    FileManager.RemoveFromDb(subitem, FileManager.historyDbInfo);
                }
            }
            else if (keepInHistory)
            {
                FileManager.UpdateHistory(txtSearch.Text, Path.GetFileName(subitem), subitem);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void yalContextStrip_Opening(object sender, CancelEventArgs e)
        {
            // only display the "Show Yal" option if the context menu is opened through the tray icon
            showYalToolStripMenuItem.Visible = !this.Bounds.Contains(Cursor.Position);
        }

        private void showYalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActivateYal();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ActivateYal();
            }
        }

        private void rebuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RebuildIndex();
        }

        private void Yal_Load(object sender, EventArgs e)
        {
            RegisterHotkey();

            bool didNotExist;
            FileManager.EnsureDbExists(out didNotExist);

            if (didNotExist)
            {
                RebuildIndex();
            }
        }

        private bool StandardSearch()
        {
            return !Properties.Settings.Default.MatchAnywhere && !Properties.Settings.Default.FuzzyMatching
                   && !Properties.Settings.Default.FuzzyMatchingPluginItems;
        }
    }
}
