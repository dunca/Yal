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
using System.Diagnostics;
using Utilities;

namespace Yal
{
    public partial class OutputWindow : Form
    {
        Launcher MainWindow { get; }

        public OutputWindow(Launcher mainWindow)
        {
            InitializeComponent();

            MainWindow = mainWindow;

            CreateLVColumns();

            listViewOutput.ShowItemToolTips = true;
        }

        private void CreateLVColumns()
        {
            //listViewOutput.HeaderStyle = ColumnHeaderStyle.None;
            var c1 = new ColumnHeader() { Name="ColName", Text="Name"};
            var c2 = new ColumnHeader() { Name = "ColFullName", Text = "FullName" };
            listViewOutput.Columns.AddRange(new ColumnHeader[] { c1, c2 });
        }

        internal void ResizeToFitContent()
        {
            // todo. mess with the width in the TileSize prop... keep count of the vert. scrollbar and increase the width of the tile when
            // there's no scrollbar
            int neededRows = Math.Min(Properties.Settings.Default.MaxVisible, listViewOutput.Items.Count);
            this.Size = new Size(this.Size.Width, (neededRows * listViewOutput.TileSize.Height) + 5); // add 5px to the heigh to compensate for borders
        }

        private void listViewOutput_MouseEnter(object sender, EventArgs e)
        {
            listViewOutput.Focus();
        }

        private void listViewOutput_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                BuildContextMenu(Cursor.Position);
            }
        }

        internal void BuildContextMenu(Point location)
        {
            var contextMenu = new ContextMenuStrip();

            var runItem = new ToolStripMenuItem("Run");
            runItem.Click += RunItem_Click;

            var runAsAdminItem = new ToolStripMenuItem("Run as administrator");
            runAsAdminItem.Click += RunAsAdminItem_Click;

            if (File.Exists(listViewOutput.SelectedItems[0].SubItems[1].Text))
            {
                var openDirItem = new ToolStripMenuItem("Open containing directory");
                openDirItem.Click += OpenDirItem_Click;
                contextMenu.Items.AddRange(new ToolStripItem[] { openDirItem, new ToolStripSeparator() });
            }

            contextMenu.Items.AddRange(new ToolStripItem[] { runItem, runAsAdminItem });
            contextMenu.Show(location);
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
