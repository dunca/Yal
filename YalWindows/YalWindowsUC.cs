using System.Windows.Forms;

namespace YalWindows
{
    public partial class YalWindowsUC : UserControl
    {
        public YalWindowsUC()
        {
            InitializeComponent();
            cbGetAppIcons.Checked = Properties.Settings.Default.GetAppIcons;
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.GetAppIcons = cbGetAppIcons.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
