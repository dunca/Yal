using System;
using System.ComponentModel;

namespace Yal
{
    class FolderToIndex
    {
        public int Depth { get; set; }
        public string Path { get; set; }
        public BindingList<string> Extensions { get; set; }

        public FolderToIndex()
        {

        }

        public FolderToIndex(string rawItem)
        {
            var split = rawItem.Split('|');
            Path = split[0];
            Depth = Convert.ToInt32(split[2]);

            Extensions = new BindingList<string>();
            foreach (var ext in split[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Extensions.Add(ext);
            }
        }
    }
}
