using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace YalClipboardHistory
{
    static class HistoryManager
    {
        private const string historyFile = "ychdb.xml";
        internal static List<string> HistoryItems { get; } = new List<string>();

        static HistoryManager()
        {
            if (!Properties.Settings.Default.StoreInDb)
            {
                File.Delete(historyFile);
            }    
            else if (File.Exists(historyFile))
            {
                var serializer = new XmlSerializer(HistoryItems.GetType());
                using (var file = File.OpenText(historyFile))
                {
                    HistoryItems = (List<string>)serializer.Deserialize(file);
                }
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

                HistoryItems.Add(data);

                if (HistoryItems.Count > Properties.Settings.Default.MaxHistorySize)
                {
                    HistoryItems.RemoveAt(HistoryItems.Count - 1);
                }
            }
        }

        internal static void UpdateLocalDb()
        {
            using (var fs = File.Open(historyFile, FileMode.Create))
            {
                var serializer = new XmlSerializer(HistoryItems.GetType());
                serializer.Serialize(fs, HistoryItems);
            }
        }
    }
}
