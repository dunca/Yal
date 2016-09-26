using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YalPath
{
    public partial class YalPathUC : UserControl
    {
        public YalPathUC()
        {
            InitializeComponent();

            cbCopyPath.Checked = Properties.Settings.Default.CopyPath;
            cbOpenPath.Checked = Properties.Settings.Default.OpenPath;
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.CopyPath = cbCopyPath.Checked;
            Properties.Settings.Default.OpenPath = cbOpenPath.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
