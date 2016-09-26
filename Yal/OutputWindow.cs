using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using Utilities;

namespace Yal
{
    public partial class OutputWindow : Form
    {
        private Yal MainWindow { get; }

        public OutputWindow(Yal mainWindow)
        {
            InitializeComponent();

            UpdateIconState();
            MainWindow = mainWindow;
            listViewOutput.ShowItemToolTips = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab && MainWindow.TabComplete())
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        internal void ResizeToFitContent()
        {
            int neededRows = Math.Min(Properties.Settings.Default.MaxVisible, listViewOutput.Items.Count);
            this.Size = new Size(this.Size.Width, (neededRows * listViewOutput.TileSize.Height) + 4); // add 4px to the heigh to compensate for borders

            // dynamically change the tile's width based on the number of items (by considering the state of the vert. scrollbar)
            //listViewOutput.TileSize = new Size(listViewOutput.Items.Count <= Properties.Settings.Default.MaxVisible ? 
            //                                   listViewOutput.Size.Width : listViewOutput.Size.Width - 17, 
            //                                   listViewOutput.TileSize.Height);
            listViewOutput.TileSize = new Size(listViewOutput.ClientSize.Width,
                                               listViewOutput.TileSize.Height);
        }

        private void listViewOutput_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                BuildContextMenu();
            }
        }

        private void listViewOutput_KeyDown(object sender, KeyEventArgs e)
        {
            char inputChar = (char)e.KeyCode;

            if (char.IsLetter(inputChar))
            {
                if (e.Modifiers == Keys.Control)
                {
                    if (e.KeyCode == Keys.O)
                    {
                        MainWindow.ShowOptionsWindow();
                    }
                    else if (e.KeyCode == Keys.A)
                    {
                        MainWindow.txtSearch.Focus();
                        MainWindow.txtSearch.SelectAll();
                    }
                    else if (listViewOutput.SelectedItems.Count != 0 && e.KeyCode == Keys.P)
                    {
                        BuildContextMenu();
                    }
                    return;
                }

                if (!Control.IsKeyLocked(Keys.CapsLock) || (Control.IsKeyLocked(Keys.CapsLock) && e.Modifiers == Keys.Shift))
                {
                    inputChar = char.ToLower(inputChar);
                }

                AlterSearchBoxText(string.Concat(MainWindow.txtSearch.Text, inputChar));

            }
            else if (e.KeyCode == Keys.Space)
            {
                AlterSearchBoxText(string.Concat(MainWindow.txtSearch.Text, ' '));
            }
            else if (e.KeyCode == Keys.Back)
            {
                AlterSearchBoxText(MainWindow.txtSearch.Text.Substring(0, MainWindow.txtSearch.Text.Length - 1));

            }
            else if (e.KeyCode == Keys.Enter)
            {
                // the user can hold CTRL+SHIFT+ENTER to run the item with elevated rights. If just SHIFT+ENTER are pressed,
                // the search term won't be saved in the history database
                MainWindow.StartSelectedItem(e.Modifiers == (Keys.Shift | Keys.Control), !(e.Modifiers == Keys.Shift));
            }
        }

        private void AlterSearchBoxText(string text, bool focus = true)
        {
            MainWindow.txtSearch.Text = text;
            MainWindow.txtSearch.SelectionStart = MainWindow.txtSearch.Text.Length;

            if (focus)
            {
                MainWindow.txtSearch.Focus();
            }
        }

        private void listViewOutput_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MainWindow.StartSelectedItem();
        }

        internal void UpdateIconState()
        {
            if (Properties.Settings.Default.ShowItemIcons)
            {
                listViewOutput.LargeImageList = imageList1;
            }
            else
            {
                listViewOutput.LargeImageList = null;
            }

            // For some reason, just changing the LargeImageList reference to null doesn't get rid of the space
            // taken up by icons. Switching the view style makes it work as expected
            listViewOutput.View = View.Details;
            listViewOutput.View = View.Tile;
        }

        internal void BuildContextMenu(Point? location = null)
        {
            if (location == null)
            {
                location = Cursor.Position;
            }

            var contextMenu = new ContextMenuStrip();

            var runItem = new ToolStripMenuItem("Run");
            runItem.Click += RunItem_Click;

            var copyNameItem = new ToolStripMenuItem("Copy name");
            copyNameItem.Click += CopyNameItem_Click;

            if (File.Exists(listViewOutput.SelectedItems[0].SubItems[1].Text))
            {
                var copyPathItem = new ToolStripMenuItem("Copy path");
                copyPathItem.Click += CopyPathItem_Click;

                var runAsAdminItem = new ToolStripMenuItem("Run as administrator");
                runAsAdminItem.Click += RunAsAdminItem_Click;

                var openDirItem = new ToolStripMenuItem("Open containing directory");
                openDirItem.Click += OpenDirItem_Click;
                contextMenu.Items.AddRange(new ToolStripItem[] { copyPathItem, runAsAdminItem, openDirItem });
            }

            contextMenu.Items.AddRange(new ToolStripItem[] { copyNameItem, runItem });
            contextMenu.Show((Point)location);
        }

        private void CopyPathItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listViewOutput.SelectedItems[0].SubItems[1].Text);
        }

        private void CopyNameItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listViewOutput.SelectedItems[0].SubItems[0].Text);
        }

        private void OpenDirItem_Click(object sender, EventArgs e)
        {
            Utils.OpenFileDirectory(listViewOutput.SelectedItems[0].SubItems[1].Text);
        }

        private void RunAsAdminItem_Click(object sender, EventArgs e)
        {
            MainWindow.StartSelectedItem(elevatedRights: true);
        }

        private void RunItem_Click(object sender, EventArgs e)
        {
            MainWindow.StartSelectedItem();
        }
    }
}
