using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace Yal
{
    static class Program
    {
        static bool hasMutex = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mutex = new Mutex(false, "6d6f9d7431bf40f4bada471eca786385");
            try
            {
                hasMutex = mutex.WaitOne(millisecondsTimeout: 0);
                StartApplication();
            }
            finally
            {
                if (hasMutex)
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        static void StartApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var application = new SingleInstanceApplication(new Yal(hasMutex));
            application.StartupNextInstance += (sender, e) => { e.BringToForeground = true; };
            application.Run(Environment.GetCommandLineArgs());
        }

        class SingleInstanceApplication : WindowsFormsApplicationBase
        {
            public SingleInstanceApplication(Form form)
            {
                MainForm = form;
                IsSingleInstance = true;
            }
        }
    }
}
