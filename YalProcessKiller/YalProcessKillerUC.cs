using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YalProcessKiller
{
    public partial class YalProcessKillerUC : UserControl
    {
        public YalProcessKillerUC()
        {
            InitializeComponent();

            cbWarnBeforeKill.Checked = Properties.Settings.Default.WarnBeforeKill;
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.WarnBeforeKill = cbWarnBeforeKill.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
