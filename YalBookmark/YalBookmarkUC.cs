using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YalBookmark
{
    public partial class YalBookmarkUC : UserControl
    {
        public YalBookmarkUC()
        {
            InitializeComponent();

            numericUpDownTruncate.Value = Properties.Settings.Default.Truncate;
            cbOpenWithProvider.Checked = Properties.Settings.Default.OpenWithProvider;
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.OpenWithProvider = cbOpenWithProvider.Checked;
            Properties.Settings.Default.Truncate = (int)numericUpDownTruncate.Value;
            Properties.Settings.Default.Save();
        }
    }
}
