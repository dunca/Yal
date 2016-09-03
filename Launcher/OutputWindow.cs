using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public partial class OutputWindow : Form
    {
        Launcher MainWindow { get; }

        public OutputWindow(Launcher mainWindow)
        {
            InitializeComponent();

            MainWindow = mainWindow;

            CreateLVColumns();
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
    }
}
