using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using Utilities;
using PluginInterfaces;

namespace YalClipboardHistory
{
    public class YalClipboardHistory : IPlugin
    {
        public string Name { get; } = "YalClipboardHistory";
        public string Version { get; } = "1.0";
        public string Description { get; } = "Easily access your clipboard's history through Yal";
        public string Activator { get; } = "cb";
        public PluginItemSortingOption SortingOption { get; } = PluginItemSortingOption.ByOriginalPosition;

        public Icon PluginIcon { get; }
        public string HelpText { get; }
        public UserControl PluginUserControl
        {
            get
            {
                if (pluginUserControl == null || pluginUserControl.IsDisposed)
                {
                    pluginUserControl = new YalClipboardHistoryUC();
                }
                return pluginUserControl;
            }
        }

        private YalClipboardHistoryUC pluginUserControl;
        private Form clipboardListener = new ClipboardListener();

        public YalClipboardHistory()
        {
            PluginIcon = Utils.GetPluginIcon(Name);

             HelpText = $@"You can access your clipboard history by
simplit typing '{Activator}'. This will provide a list of
history items, with the most recent ones places at
the top of the list. To copy an item from the history
you can either hit Enter or double click the item.

{Name} can save your clipboard history
between sessions, if the
'{(PluginUserControl as YalClipboardHistoryUC).cbStoreInDb.Text}' option is checked.
";
        }

        ~YalClipboardHistory()
        {
            if (Properties.Settings.Default.StoreInDb)
            {
                HistoryManager.UpdateLocalDb();
            }
        }

        public void SaveSettings()
        {
            pluginUserControl.SaveSettings();
        }

        public List<PluginItem> GetItems(string userInput)
        {
           return HistoryManager.HistoryItems.Count > 0 ? HistoryManager.HistoryItems.Select(item => new PluginItem()
           {
               Name = item, AlternateInfo = string.Join(" ", Activator, item)
           }).ToList() : null;
        }

        public void HandleExecution(string input)
        {
            Clipboard.SetText(input.Substring(input.IndexOf(" ") + 1));
        }
    }
}
