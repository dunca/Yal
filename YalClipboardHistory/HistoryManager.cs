using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace YalClipboardHistory
{
    class HistoryManager
    {
        private const string historyFile = "ychdb.dat";
        internal static LinkedList<string> HistoryItems { get; }

        static HistoryManager()
        {
            HistoryItems = new LinkedList<string>();
            if (Properties.Settings.Default.StoreInDb && File.Exists(historyFile))
            {
                foreach (var line in File.ReadAllLines(historyFile))
                {
                    HistoryItems.AddLast(line);
                }
            }
        }

        ~HistoryManager()
        {
            if (Properties.Settings.Default.StoreInDb)
            {
                UpdateLocalDb();
            }
            else
            {
                File.Delete(historyFile);
            }
        }

        internal static void UpdateItemList()
        {
            IDataObject clipboardData = Clipboard.GetDataObject();
            
            if (clipboardData.GetDataPresent(DataFormats.UnicodeText))
            {
                var data = clipboardData.GetData(DataFormats.UnicodeText) as string;

                if (HistoryItems.Contains(data))
                {
                    HistoryItems.Remove(data);
                }

                HistoryItems.AddFirst(data);

                if (HistoryItems.Count > Properties.Settings.Default.MaxHistorySize)
                {
                    HistoryItems.RemoveLast();
                }
            }
        }

        private void UpdateLocalDb()
        {
            File.WriteAllLines(historyFile, HistoryItems);
        }
    }
}
