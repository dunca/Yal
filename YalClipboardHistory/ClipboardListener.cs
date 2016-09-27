using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YalClipboardHistory
{
    class ClipboardListener : Form
    {
        // message received when the content of the clipboard is changed
        // https://msdn.microsoft.com/en-us/library/ms649021(v=vs.85).aspx
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        public ClipboardListener()
        {
            User32Methods.MakeMessageOnlyWindow(this.Handle);
            User32Methods.AddClipboardFormatListener(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                HistoryManager.UpdateItemList();
                return;
            }
            base.WndProc(ref m);
        }
    }
}
