using System;
using System.Windows.Forms;

namespace YalWeb
{
    public partial class YalWebUC : UserControl
    {
        YalWeb pluginInstance;

        public YalWebUC(YalWeb pluginInstance)
        {
            InitializeComponent();

            ParseEntries();

            this.pluginInstance = pluginInstance;
        }

        private void ParseEntries()
        {
            bool defaultExists = false;
            foreach (string line in Properties.Settings.Default.Entries)
            {
                var split = line.Split('|');
                var name = split[0];
                var url = split[1];
                listViewEntries.Items.Add(new ListViewItem(new string[] { name, url }));

                if (name == Properties.Settings.Default.DefaultEntry)
                {
                    defaultExists = true;
                    SetDefaultRow(name);
                }
            }
            if (!defaultExists && Properties.Settings.Default.DefaultEntry != "")
            { // just in case the default is set to something it doesn't actually exist in the list of available options
                Properties.Settings.Default.DefaultEntry = "";
            }
        }

        private void SetDefaultRow(string name)
        {
           lblDefault.Text = name;
           Properties.Settings.Default.DefaultEntry = name;
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.Entries.Clear();
            foreach (ListViewItem item in listViewEntries.Items)
            {
                Properties.Settings.Default.Entries.Add(string.Join("|", item.SubItems[0].Text, item.SubItems[1].Text));
            }
            pluginInstance.UpdateEntries();
            Properties.Settings.Default.Save();
        }

        private void btnAddEntry_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string url = txtUrl.Text;
            if (name == string.Empty || url == string.Empty || name == "s")
            {
                MessageBox.Show($"Cell value is empty or uses an already assigned value", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            listViewEntries.Items.Add(new ListViewItem(new string[] { name, url }));
        }

        private void listViewEntries_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listViewEntries.FocusedItem.SubItems[0].Text);
        }

        private void copyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listViewEntries.FocusedItem.SubItems[1].Text);
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewEntries.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listViewEntries.FocusedItem;
                if (lvi.SubItems[0].Text == Properties.Settings.Default.DefaultEntry || listViewEntries.Items.Count == 1)
                {
                    lblDefault.Text = "not set";
                    Properties.Settings.Default.DefaultEntry = "";
                }
                listViewEntries.Items.Remove(lvi);
            }
        }

        private void setAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewEntries.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Select an item first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetDefaultRow(listViewEntries.FocusedItem.SubItems[0].Text);
        }
    }
}
