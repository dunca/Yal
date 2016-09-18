using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YalCalc
{
    public partial class YalCalcUC : UserControl
    {
        public YalCalcUC()
        {
            InitializeComponent();

            numericUpDownDecimalPlaces.Value = Properties.Settings.Default.DecimalPlaces;
            cbReplaceClipboard.Checked = Properties.Settings.Default.ReplaceClipboard;
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.DecimalPlaces = (int)numericUpDownDecimalPlaces.Value;
            Properties.Settings.Default.ReplaceClipboard = cbReplaceClipboard.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
