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
using System.Text.RegularExpressions;

using PluginInterfaces;

namespace Yal
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms646309(v=vs.85).aspx
    public enum FsModifier
    {
        ALT = 0x0001,
        CTRL = 0x0002,
        SHIFT = 0x0004,
        WIN = 0x0008
    }

    public partial class Yal : Form
    {
        const int HOTKEY_REG_ID = 1;
        const int WM_HOTKEY = 0x312;  // message code that occurs when hotkeys are detected
        
        Timer timerSearchDelay;
        Timer timerTrimHistory;
        bool LmbIsDown { get; set; }
        Form OptionsForm { get; set; }
        Point LastPointerLocation { get; set; }

        OutputWindow outputWindow;
        internal Options optionsWindow;
        internal List<IPlugin> PluginInstances;

        private Dictionary<string, string> pluginItemsWithActivators = new Dictionary<string, string>();
        private const string attachTemplate = "attach database '{0}' as {1}";
        private const string pluginTableSchema = "create table if not exists PLUGIN_ITEMS (ITEM_NAME string, PLUGIN_NAME string, ADDITIONAL_INFO string)";
        private const string pluginInsertString = "insert into PLUGIN_ITEMS (ITEM_NAME, PLUGIN_NAME, ADDITIONAL_INFO) values (@item_name, @plugin_name, @additional_info)";
        private const string itemQueryString = @"select distinct ITEM_NAME, OTHER_INFO from 
                                               (select ITEM_NAME, OTHER_INFO, HITS from HISTORY_CATALOG where SNIPPET like @snippet
                                               union
                                               select NAME as ITEM_NAME, FULLPATH as OTHER_INFO, @file_priority as HITS from INDEX_CATALOG where NAME like @pattern
                                               union
                                               select ITEM_NAME, PLUGIN_NAME as OTHER_INFO, 0 as HITS from PLUGIN_ITEMS where ITEM_NAME like @pattern
                                               order by HITS desc, NAME asc) limit @limit";
        private SQLiteConnection pluginTempConnection = new SQLiteConnection("FullUri=file::memory:?cache=shared;Version=3;");

        public Yal()
        {
            InitializeComponent();

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

            PluginInstances = PluginLoader.InstantiatePlugins(PluginLoader.Load("plugins"));

            pluginTempConnection.Open();
            (new SQLiteCommand(pluginTableSchema, pluginTempConnection)).ExecuteNonQuery();
        }

        internal void ShowOptionsWindow()
        {
            if (optionsWindow == null)
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
                if (this.Visible)
                {
                    this.Hide();
                }
                else
                {
                    ActivateYal();
                }
            }
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
            if ((LmbIsDown && !Properties.Settings.Default.MoveWithCtrl) ||
                (LmbIsDown && Properties.Settings.Default.MoveWithCtrl &&
                (Control.ModifierKeys == Keys.Control)))
            {
                this.Location = new Point(this.Location.X + (e.X - LastPointerLocation.X),
                                          this.Location.Y + (e.Y - LastPointerLocation.Y));
            }
        }

        private void Yal_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LmbIsDown = true;
                LastPointerLocation = e.Location;
                this.Cursor = Cursors.SizeAll;  // the little crosshair like cursor
            }
        }

        private void Yal_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LmbIsDown = false;
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
                if (!outputWindow.Visible)
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
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.SearchDelay <= 100)
            {
                PerformSearch();
            }
            else
            {
                timerSearchDelay.Stop();
                timerSearchDelay.Interval = Properties.Settings.Default.SearchDelay;
                timerSearchDelay.Start();
            }
        }

        private void SearchDelayTimer_Tick(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            timerSearchDelay.Stop();
            outputWindow.imageList1.Images?.Clear();
            outputWindow.listViewOutput.Items.Clear();

            if (txtSearch.Text != string.Empty)
            {
                int pluginItemCount = 0;
                foreach (var plugin in PluginInstances)
                {
                    if (Properties.Settings.Default.DisabledPlugins.Contains(plugin.Name))
                    {
                        continue;
                    }

                    string[] itemInfo;
                    string[] pluginItems = plugin.GetResults(txtSearch.Text, out itemInfo);

                    if (!plugin.FileLikeOutput)
                    {
                        var pattern = Properties.Settings.Default.FuzzyMatching ? string.Concat(txtSearch.Text.Select(c => string.Concat(c, ".*"))) : txtSearch.Text;
                        var regex = new Regex(Properties.Settings.Default.MatchAnywhere ? pattern : string.Concat("^", pattern));
                        foreach (var pluginItem in pluginItems)
                        {
                            //if (pluginItem.StartsWith(txtSearch.Text.IndexOf(" ") == -1 ? txtSearch.Text : txtSearch.Text.Split()[0]))
                            if (regex.IsMatch(pluginItem))
                            {
                                pluginItemsWithActivators.Add(pluginItem, plugin.Name);
                            }
                        }
                        continue;
                    }

                    //// give higher priority to file-like plugins (used to order items in the SQL query)
                    //var additionalInfo = plugin.FileLikeOutput ? 0 : -1;

                    foreach (var pluginItem in pluginItems)
                    {
                        pluginItemCount++;
                        var command = new SQLiteCommand(pluginInsertString, pluginTempConnection);
                        command.Parameters.AddWithValue("@item_name", pluginItem);
                        command.Parameters.AddWithValue("@plugin_name", plugin.Name);
                        command.Parameters.AddWithValue("@additional_info", 0);
                        command.ExecuteNonQuery();
                    }
                }

                using (var connection = FileManager.GetDbConnection(FileManager.historyDbInfo))
                {
                    (new SQLiteCommand(string.Format(attachTemplate, "file::memory:?cache=shared", "PLUGIN_ITEMS"), connection)).ExecuteNonQuery();
                    (new SQLiteCommand(string.Format(attachTemplate, FileManager.indexDbInfo.fileName, "INDEX_DB"), connection)).ExecuteNonQuery();

                    var command = new SQLiteCommand(itemQueryString, connection);

                    string pattern = Properties.Settings.Default.FuzzyMatching ? string.Concat(txtSearch.Text.Select(c => string.Concat(c, "%"))) :
                                                                                 string.Concat(txtSearch.Text, "%");
                    pattern = string.Concat(Properties.Settings.Default.MatchAnywhere ? "%" : "", pattern);

                    command.Parameters.AddWithValue("@file_priority", Properties.Settings.Default.PluginItemsFirst ? 0 : 1);
                    command.Parameters.AddWithValue("@limit", Properties.Settings.Default.MaxItems);
                    command.Parameters.AddWithValue("@snippet", string.Concat(txtSearch.Text, "%"));
                    command.Parameters.AddWithValue("@pattern", pattern);
                    var reader = command.ExecuteReader();

                    int iconIndex = 0;
                    while (reader.Read())
                    {
                        var itemName = reader["ITEM_NAME"].ToString();
                        var otherInfo = reader["OTHER_INFO"].ToString();

                        ListViewItem lvi = null;
                        IPlugin pluginInstance = PluginInstances.Find(plugin => plugin.Name == otherInfo);
                        if (pluginInstance != null)
                        {
                            lvi = GetPluginLvi(pluginInstance, itemName, otherInfo, ref iconIndex);
                        }
                        else
                        {
                            //  most likely a file
                            if (!Properties.Settings.Default.ExtensionInFileName)
                            {
                                itemName = Path.GetFileNameWithoutExtension(otherInfo);
                            }

                            Icon icon;
                            if (FileManager.GetFileIcon(otherInfo, out icon))
                            {
                                lvi = new ListViewItem(new string[] { itemName, otherInfo }, imageIndex: iconIndex) { ToolTipText = otherInfo };
                                outputWindow.imageList1.Images.Add(icon);
                                iconIndex++;
                            }
                        }
                        lvi.SubItems[0].Text = TrimStringIfNeeded(itemName);
                        outputWindow.listViewOutput.Items.Add(lvi);
                    }
                    foreach (var item in pluginItemsWithActivators)
                    {
                        var itemName = TrimStringIfNeeded(item.Key);
                        ListViewItem lvi = GetPluginLvi(PluginInstances.Find(p => p.Name == item.Value), itemName, 
                                                        item.Value, ref iconIndex);
                        outputWindow.listViewOutput.Items.Add(lvi);
                    }
                    pluginItemsWithActivators.Clear();
                    (new SQLiteCommand("delete from PLUGIN_ITEMS", pluginTempConnection)).ExecuteNonQuery();
                }

                if (outputWindow.listViewOutput.Items.Count > 0)
                {
                    outputWindow.Show(); // Show() it first, so that the listview's ClientSize property gets updated

                    outputWindow.listViewOutput.Items[0].Selected = true;
                    outputWindow.ResizeToFitContent();
                    this.txtSearch.Focus();  // Show()-ing a window focuses on it by default. We don't want that in this case;
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

        private ListViewItem GetPluginLvi(IPlugin pluginInstance, string itemName, string otherInfo, ref int iconIndex)
        {
            var lvi = new ListViewItem(new string[] { itemName, otherInfo });
            if (pluginInstance.PluginIcon != null)
            {
                outputWindow.imageList1.Images.Add(pluginInstance.PluginIcon);
                lvi.ImageIndex = iconIndex;
                iconIndex++;
            }
            return lvi;
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

            this.Hide();
            outputWindow.Hide();

            // the first item in each row
            string item = outputWindow.listViewOutput.SelectedItems[0].SubItems[0].Text;
            // the 2nd item in each row. Usually a plugin name or a full file path
            string subitem = outputWindow.listViewOutput.SelectedItems[0].SubItems[1].Text;

            var plugin = PluginInstances.Find(pluginInstance => pluginInstance.Name == subitem);

            if (plugin != null)
            {
                plugin.HandleExecution(item);

                if (plugin.FileLikeOutput && Properties.Settings.Default.PluginSelectionsInHistory)
                {
                    FileManager.UpdateHistory(txtSearch.Text, item, plugin.Name);
                }
                return;
            }

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
    }
}
