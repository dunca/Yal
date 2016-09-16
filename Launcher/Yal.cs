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

    public partial class Launcher : Form
    {
        const int HOTKEY_REG_ID = 1;
        const int WM_HOTKEY = 0x312;  // message code that occurs when hotkeys are detected
        
        Timer searchDelayTimer;
        Timer trimHistoryTimer;
        bool LmbIsDown { get; set; }
        Form OptionsForm { get; set; }
        Point LastPointerLocation { get; set; }

        OutputWindow outputWindow;
        internal Options optionsWindow;
        internal List<IPlugin> PluginInstances;

        public Launcher()
        {
            InitializeComponent();
            
            outputWindow = new OutputWindow(this);
            outputWindow.listViewOutput.KeyDown += outputWindow_listViewOutput_KeyDown;
            outputWindow.listViewOutput.MouseDoubleClick += outputWindow_listViewOutput_MouseDoubleClick;

            ManageAutoIndexingTimer();
            UpdateWindowLocation();
            UpdateWindowLooks();
            RegisterHotkey();
            
            searchDelayTimer = new Timer();
            searchDelayTimer.Tick += SearchDelayTimer_Tick;

            trimHistoryTimer = new Timer();
            trimHistoryTimer.Interval = 60 * 60 * 1000;  // hourly
            trimHistoryTimer.Tick += TrimHistoryTimer_Tick;

            Properties.Settings.Default.FoldersToIndex = FileManager.ProcessRawPaths();

            PluginInstances = PluginLoader.InstantiatePlugins(PluginLoader.Load("plugins"));
        }

        private void ShowOptionsWindow()
        {
            if (optionsWindow == null)
            {
                optionsWindow = new Options(this);
            }
            optionsWindow.Show();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (outputWindow.listViewOutput.SelectedItems.Count != 0)
            {
                if (keyData == (Keys.Control | Keys.D))
                {
                    Utils.OpenFileDirectory(outputWindow.listViewOutput.SelectedItems[0].SubItems[1].Text);
                    return true;
                }
                else if (keyData == (Keys.Control | Keys.P))
                {
                    ShowItemContextMenu();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowItemContextMenu()
        {
            ListViewItem item = outputWindow.listViewOutput.SelectedItems[0];
            Point location = new Point(outputWindow.Location.X + item.Position.X + (outputWindow.listViewOutput.TileSize.Width / 2),
                                       outputWindow.Location.Y + item.Position.Y + (outputWindow.listViewOutput.TileSize.Height / 2));
            outputWindow.BuildContextMenu(location);
        }

        private void TrimHistoryTimer_Tick(object sender, EventArgs e)
        {
            FileManager.TrimHistory();
        }

        internal void ManageAutoIndexingTimer()
        {
            //int autoIndexingInterval;
            //if (int.TryParse(Properties.Settings.Default.IndexingInterval, out autoIndexingInterval))
            //{
            //    timerAutoIndexing.Stop();
            //    // the interval is in milliseconds so a conversion is needed
            //    timerAutoIndexing.Interval = autoIndexingInterval * 1000 * 60;
            //    timerAutoIndexing.Tick -= TimerAutoIndexing_Tick;
            //    timerAutoIndexing.Tick += TimerAutoIndexing_Tick;
            //    timerAutoIndexing.Start();
            //}
            //else
            //{
            //    timerAutoIndexing.Stop();
            //}
            if (Properties.Settings.Default.AutoIndexing)
            {
                timerAutoIndexing.Stop();
                // the interval is in milliseconds so a conversion is needed
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
            // make sure that enough minutes have already passed away since the last indexing. We use this in order to avoid
            // multiple/unecessary indexing actions in case the handler is registered multiple times
            //if (Properties.Settings.Default.DateLastIndexed.AddMinutes(double.Parse(Properties.Settings.Default.IndexingInterval)) <
            //    DateTime.Now)
            {
                FileManager.RebuildIndex();
                optionsWindow?.UpdateIndexingStatus();
            }
        }

        internal void UpdateHotkey()
        {
            // re-registering picks up updated properties set by the uses throught the Options form
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
            RegisterHotKey(this.Handle, HOTKEY_REG_ID, (uint)Properties.Settings.Default.FocusModifier,
                           (uint)Properties.Settings.Default.FocusKey);
        }

        private void UnregisterHotkey()
        {
            UnregisterHotKey(this.Handle, HOTKEY_REG_ID);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // focus on our form when the hotkey (Win+Z by default) is pressed
            if (m.Msg == WM_HOTKEY)
            {
                if (this.Visible)
                {
                    this.Hide();
                }
                else
                {
                    ActivateLauncher();
                }
            }
        }

        private void ActivateLauncher()
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

        private void Launcher_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotkey();
            Properties.Settings.Default.Location = Location;
            Properties.Settings.Default.Save();
        }

        private void Launcher_Move(object sender, EventArgs e)
        {
            outputWindow.Location = new Point(this.Location.X, this.Location.Y + this.Height);
        }

        private void Launcher_Shown(object sender, EventArgs e)
        {
            ActivateLauncher();
        }

        private void Launcher_MouseMove(object sender, MouseEventArgs e)
        {
            if ((LmbIsDown && !Properties.Settings.Default.MoveWithCtrl) ||
                (LmbIsDown && Properties.Settings.Default.MoveWithCtrl &&
                (Control.ModifierKeys == Keys.Control)))
            {
                this.Cursor = Cursors.SizeAll;  // the little crosshair like cursor
                this.Location = new Point(this.Location.X + (e.X - LastPointerLocation.X),
                                          this.Location.Y + (e.Y - LastPointerLocation.Y));
            }
        }

        private void Launcher_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LmbIsDown = true;
                LastPointerLocation = e.Location;
            }
        }

        private void Launcher_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LmbIsDown = false;
                this.Cursor = Cursors.Default;  // we only want the crosshair cursor when moving the window
                //AlignHorizontally(this);
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOptionsWindow();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                //if (outputWindow.listViewOutput.Items.Count > 1)
                //{
                ////outputWindow.listViewOutput.Items[0].Selected = false;
                ////outputWindow.listViewOutput.Items[1].Selected = true;

                //int indexOfSelected = outputWindow.listViewOutput.Items.IndexOf(outputWindow.listViewOutput.SelectedItems[0]);
                //outputWindow.listViewOutput.Items[indexOfSelected].Selected = false;
                //outputWindow.listViewOutput.Items[++indexOfSelected].Selected = true;
                //outputWindow.listViewOutput.FocusedItem = outputWindow.listViewOutput.Items[indexOfSelected];
                //}
                if (outputWindow.Visible == false)
                {
                    outputWindow.Show();
                }
                outputWindow.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                StartSelectedItem();
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
                searchDelayTimer.Stop();
                searchDelayTimer.Interval = Properties.Settings.Default.SearchDelay;
                searchDelayTimer.Start();
            }
        }

        private void SearchDelayTimer_Tick(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            //foreach (var plugin in PluginInstances)
            //{
            //    string ret;
            //    if (plugin.TryParseInput(txtSearch.Text, out ret))
            //    {
            //        outputWindow.listViewOutput.Items.Insert(0, ret);
            //        outputWindow.Visible = true;
            //    }
            //}

            if (txtSearch.Text != string.Empty)
            {
                outputWindow.imageList1.Images?.Clear();
                outputWindow.listViewOutput.Items.Clear();

                int iconIndex = 0;
                foreach (var plugin in PluginInstances)
                {
                    //if (plugin.Activators.Any(activator => txtSearch.Text.StartsWith(activator, StringComparison.InvariantCultureIgnoreCase) 
                    //                                       || activator.StartsWith(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase)))
                    //if ((!plugin.FileLikeOutput && plugin.Activators.Any(activator => txtSearch.Text.StartsWith(activator, StringComparison.InvariantCultureIgnoreCase)))
                    //    || (plugin.FileLikeOutput && plugin.Activators.Any(activator => Properties.Settings.Default.MatchAnywhere ? activator.IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) > -1 : 
                    //                                                                                                                activator.StartsWith(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase))))

                    if ((plugin.CouldProvideResults(txtSearch.Text, Properties.Settings.Default.MatchAnywhere, Properties.Settings.Default.FuzzyMatching)))
                    {
                        string[] items = plugin.GetResults(txtSearch.Text, Properties.Settings.Default.MatchAnywhere,
                                                           Properties.Settings.Default.FuzzyMatching);
                        if (items.Length > 0)
                        {
                            int count = 1;
                            foreach (var item in items)
                            {
                                var lvi = new ListViewItem(new string[] { item, plugin.Name });
                                if (plugin.PluginIcon != null)
                                {
                                    outputWindow.imageList1.Images.Add(plugin.PluginIcon);
                                    lvi.ImageIndex = iconIndex;
                                    iconIndex++;
                                }
                                outputWindow.listViewOutput.Items.Add(lvi);

                                if (count == Properties.Settings.Default.MaxVisiblePluginItems)
                                {
                                    break;
                                }
                                count++;
                            }
                        }
                    }
                }

                if (FileManager.QueryIndexDb(txtSearch.Text, Properties.Settings.Default.MaxFetched,
                                             outputWindow.listViewOutput.Items,
                                             outputWindow.imageList1.Images))
                {
                    outputWindow.listViewOutput.Items[0].Selected = true;
                    outputWindow.ResizeToFitContent();
                    outputWindow.Show();
                    this.txtSearch.Focus();  // 'Show()'-ing a window focuses on it by default. We don't want that in this case;
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

            searchDelayTimer.Stop();
        }

        private void outputWindow_listViewOutput_KeyDown(object sender, KeyEventArgs e)
        {
            //bool isAlpha = char.IsLetter((char)e.KeyCode);
            bool isAlpha = e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z;
            if (isAlpha)
            {
                if (e.Modifiers == Keys.Control)
                {
                    if (e.KeyCode == Keys.O)
                    {
                        ShowOptionsWindow();
                    }
                    else if (outputWindow.listViewOutput.SelectedItems.Count != 0)
                    {
                        if (e.KeyCode == Keys.D)
                        {
                            Utils.OpenFileDirectory(outputWindow.listViewOutput.SelectedItems[0].SubItems[1].Text);
                        }
                        else if (e.KeyCode == Keys.P)
                        {
                            ShowItemContextMenu();
                        }
                    }
                    return;
                }

                char pressed = (char)e.KeyCode;
                if (!Control.IsKeyLocked(Keys.CapsLock) ||
                    (Control.IsKeyLocked(Keys.CapsLock) && e.Shift))
                {
                    pressed = char.ToLower(pressed);
                }
                txtSearch.Text += pressed;
                txtSearch.SelectionStart = txtSearch.Text.Length;
                txtSearch.Focus();
            }
            else if (e.KeyCode == Keys.Back) // backspace
            {
                txtSearch.Text = txtSearch.Text.Substring(0, txtSearch.Text.Length - 1);
                txtSearch.SelectionStart = txtSearch.Text.Length;
                txtSearch.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                // the user can hold CTRL+SHIFT+ENTER to run the item with elevated rights
                StartSelectedItem(e.Modifiers == (Keys.Shift | Keys.Control));
            }
        }

        private void outputWindow_listViewOutput_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            StartSelectedItem();
        }

        internal void StartSelectedItem(bool elevatedRights = false)
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
            else
            {
                FileManager.UpdateHistory(txtSearch.Text, Path.GetFileName(filePath), filePath);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void launcherContextStrip_Opening(object sender, CancelEventArgs e)
        {
            // only display the "Show Yal" option if the context menu is opened through the
            // tray icon
            showLauncherToolStripMenuItem.Visible = !this.Bounds.Contains(Cursor.Position);
        }

        private void showLauncherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActivateLauncher();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ActivateLauncher();
            }
        }

        private void rebuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileManager.RebuildIndex();
        }
    }
}
