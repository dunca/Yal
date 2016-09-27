﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using Utilities;
using PluginInterfaces;

namespace YalClipboardHistory
{
    public class YalClipboardHistory : IPlugin
    {
        public string Name { get; } = "YalClipboardHistory";
        public string Version { get; } = "1.0";
        public string Description { get; } = "Easily access your clipboard's history through Yal";
        public bool RequiresActivator { get; } = false;

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
        
        // This object isn't actually used. An instance is created so that it's destructor is called
        private HistoryManager _ = new HistoryManager();
        private Form clipboardListener = new ClipboardListener();

        public YalClipboardHistory()
        {
            PluginIcon = Utils.GetPluginIcon(Name);

             HelpText = $@"You can access your clipboard history by
simplit typing 'cb'. This will provide a list of
history items, with the most recent ones places at
the top of the list. To copy an item from the history
you can either hit Enter or double click the item.

{Name} can save your clipboard history
between sessions, if the
'{(PluginUserControl as YalClipboardHistoryUC).cbStoreInDb.Text}' option is checked.
";
        }

        public void SaveSettings()
        {
            pluginUserControl.SaveSettings();
        }

        public string[] GetResults(string input, out string[] itemInfo)
        {
            itemInfo = null;
            string[] result = null;

            if (HistoryManager.HistoryItems.Count > 0)
            {
                result = HistoryManager.HistoryItems.Select(item => string.Concat("cb ", item)).ToArray();
            }

            return result;
        }

        public void HandleExecution(string input)
        {
            Clipboard.SetText(input.Substring(input.IndexOf(" ") + 1));
        }
    }
}