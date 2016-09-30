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
            foreach (string line in Properties.Settings.Default.Entries)
            {
                var split = line.Split('|');
                var name = split[0];
                var url = split[1];
                listViewEntries.Items.Add(new ListViewItem(new string[] { name, url }));
            }
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.Entries.Clear();
            foreach (ListViewItem item in listViewEntries.Items)
            {
                Properties.Settings.Default.Entries.Add(string.Join("|", item.SubItems[0].Text, item.SubItems[1].Text));
            }
            pluginInstance.PopulateEntries();
            Properties.Settings.Default.Save();
        }

        private void btnAddEntry_Click(object sender, EventArgs e)
        {
            string errorMessage = null;
            string name = txtName.Text;
            string url = txtUrl.Text;

            if (name == "" || url == "")
            {
                errorMessage = "Cell values can't be empty";
            }
            else if (!name.StartsWith(pluginInstance.Activator))
            {
                errorMessage = $"Every name command must start with the '{pluginInstance.Activator}' activator";
            }
            else
            {
                foreach (ListViewItem lvi in listViewEntries.Items)
                {
                    if (lvi.SubItems[0].Text == name)
                    {
                        errorMessage = $"An entry named '{name}' already exists";
                        break;
                    }
                }
            }

            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, "Error",
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
                listViewEntries.Items.Remove(listViewEntries.FocusedItem);
            }
        }

        private void setAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewEntries.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select an item first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
