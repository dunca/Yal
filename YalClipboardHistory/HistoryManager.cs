using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace YalClipboardHistory
{
    class HistoryManager
    {
        private readonly string historyFile;
        internal static List<string> HistoryItems { get; set; } = new List<string>();

        public HistoryManager(string pluginDirectoryPath)
        {
            historyFile = Path.Combine(pluginDirectoryPath, "clipboard_database.xml");

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

        internal void UpdateLocalDb()
        {
            using (var fs = File.Open(historyFile, FileMode.Create))
            {
                var serializer = new XmlSerializer(HistoryItems.GetType());
                serializer.Serialize(fs, HistoryItems);
            }
        }
    }
}
