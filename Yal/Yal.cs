using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PluginInterfaces;
using Utilities;

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

        public Yal()
        {
            Directory.SetCurrentDirectory(Application.StartupPath);

            InitializeComponent();
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

            PluginInstances = PluginLoader.InstantiatePlugins(PluginLoader.Load("plugins"));
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

            if (txtSearch.Text != string.Empty)
            {
                outputWindow.imageList1.Images?.Clear();
                outputWindow.listViewOutput.Items.Clear();

                int iconIndex = 0;
                int pluginItemCount = 0;
                foreach (var plugin in PluginInstances)
                {
                    if ((plugin.CouldProvideResults(txtSearch.Text, Properties.Settings.Default.MatchAnywhere, Properties.Settings.Default.FuzzyMatching)))
                    {
                        string[] items = plugin.GetResults(txtSearch.Text, Properties.Settings.Default.MatchAnywhere,
                                                           Properties.Settings.Default.FuzzyMatching);
                        foreach (var item in items)
                        {
                            pluginItemCount++;
                            var lvi = new ListViewItem(new string[] { item, plugin.Name });
                            if (plugin.PluginIcon != null)
                            {
                                outputWindow.imageList1.Images.Add(plugin.PluginIcon);
                                lvi.ImageIndex = iconIndex;
                                iconIndex++;
                            }
                            outputWindow.listViewOutput.Items.Add(lvi);

                            if (pluginItemCount == Properties.Settings.Default.MaxPluginItems)
                            {
                                goto DBQuery;
                            }
                        }
                    }
                }

                DBQuery:
                if (FileManager.QueryIndexDb(txtSearch.Text, outputWindow.listViewOutput.Items, outputWindow.imageList1.Images))
                {
                    outputWindow.listViewOutput.Items[0].Selected = true;
                    outputWindow.Show();
                    outputWindow.ResizeToFitContent();  // Show() it first, so that the listview's ClientSize property gets updated
                    this.txtSearch.Focus();  // Show()-ing a window focuses on it by default. We don't want that in this case;
                }

                if (outputWindow.listViewOutput.Items.Count == 0)
                {
                    outputWindow.Hide();
                }
            }
            else
            {
                outputWindow.Hide();
            }
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

            // the 2nd item in each row. Usually a plugin name or a file path
            string subitem = outputWindow.listViewOutput.SelectedItems[0].SubItems[1].Text;
            foreach (var plugin in PluginInstances)
            {
                if (subitem == plugin.Name)
                {
                    plugin.HandleExecution(item);

                    if (plugin.FileLikeOutput && Properties.Settings.Default.PluginSelectionsInHistory)
                    {
                        FileManager.UpdateHistory(txtSearch.Text, item, plugin.Name);
                    }
                    return;
                }
            }

            // get the full path of the currently selected item
            string filePath = outputWindow.listViewOutput.SelectedItems[0].SubItems[1].Text;
            var startInfo = new ProcessStartInfo(filePath);

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
                    FileManager.RemoveFromDb(filePath, FileManager.indexDbInfo);
                    FileManager.RemoveFromDb(filePath, FileManager.historyDbInfo);
                }
            }
            else if (keepInHistory)
            {
                FileManager.UpdateHistory(txtSearch.Text, Path.GetFileName(filePath), filePath);
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
