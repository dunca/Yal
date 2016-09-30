using System;
using System.Runtime.InteropServices;

namespace YalWindows
{
    static class User32Methods
    {
        private const byte WINDOW_RESTORE = 9;

        // http://josephgozlan.blogspot.ro/2013/02/c-bring-another-application-to.html
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        internal static void ActivateWindow(IntPtr hWnd)
        {
            if (IsIconic(hWnd))
            {
                ShowWindow(hWnd, WINDOW_RESTORE);
            }
            SetForegroundWindow(hWnd);
        }
    }
}
