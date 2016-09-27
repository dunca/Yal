using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YalClipboardHistory
{
    internal static class User32Methods
    {
        // http://pinvoke.net/default.aspx/Constants/HWND_MESSAGE.html
        // https://msdn.microsoft.com/en-us/library/ms632599(VS.85).aspx#message_only
        private static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // https://msdn.microsoft.com/en-us/library/ms649033(VS.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AddClipboardFormatListener(IntPtr hwnd);

        // https://msdn.microsoft.com/en-us/library/ms649050(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        // https://msdn.microsoft.com/en-us/library/ms633541(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// Turns the window identified by 'windowHandle' in a message only window, a window that's not 
        /// visible. It can only send/receive messages
        /// </summary>
        /// <param name="windowHandle"></param>
        internal static void MakeMessageOnlyWindow(IntPtr windowHandle)
        {
            SetParent(windowHandle, HWND_MESSAGE);
        }
    }
}
