using System.Windows.Forms;

namespace YalProcessKiller
{
    public partial class YalProcessKillerUC : UserControl
    {
        public YalProcessKillerUC()
        {
            InitializeComponent();

            cbWarnBeforeKill.Checked = Properties.Settings.Default.WarnBeforeKill;
            cbGetProcessIcons.Checked = Properties.Settings.Default.GetProcessIcons;
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.WarnBeforeKill = cbWarnBeforeKill.Checked;
            Properties.Settings.Default.GetProcessIcons = cbGetProcessIcons.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
