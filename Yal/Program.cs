using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.MemoryMappedFiles;

using Utilities;

namespace Yal
{
    static class Program
    {
        static int byteSize = IntPtr.Size;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool hasMutex = false;
            var mutex = new Mutex(false, "6d6f9d7431bf40f4bada471eca786385");
            const string memoryMappedFileName = "e4288b924ac487e93faf019b1277905";

            using (var memoryMappedFile = MemoryMappedFile.CreateOrOpen(memoryMappedFileName, byteSize))
            {
                try
                {
                    hasMutex = mutex.WaitOne(0, false);
                    if (hasMutex)
                    {
                        StartApplication(memoryMappedFile);
                    }
                    else
                    {
                        using (var accessor = memoryMappedFile.CreateViewAccessor())
                        {
                            var existingProcessId = accessor.ReadInt32(0);
                            var existingProcess = Process.GetProcessById(existingProcessId);

                            // we're using FindWindow because existingProcess.MainWindowHandle returns 0 for forms
                            // with "ShowInTaskbar = false"
                            var windowHandle = Utils.FindWindow(null, existingProcess.MainWindowTitle);

                            if (windowHandle != IntPtr.Zero)
                            {
                                Utils.ActivateWindowByHandle(windowHandle);
                            }
                            else
                            {
                                if (MessageBox.Show("Something went wrong with the launcher, would you like to restart it?",
                                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    try
                                    {
                                        existingProcess.Kill();
                                    }
                                    catch
                                    {
                                        // probably terminating or already terminated
                                    }

                                    StartApplication(memoryMappedFile);
                                }
                            }
                        }
                    }
                }
                catch (AbandonedMutexException)
                {
                    StartApplication(memoryMappedFile);
                }
                finally
                {
                    if (hasMutex)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
        }

        static void StartApplication(MemoryMappedFile memoryMappedFile)
        {
            using (var accessor = memoryMappedFile.CreateViewAccessor())
            {
                accessor.Write(0, Process.GetCurrentProcess().Id);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Yal());
        }
    }
}
