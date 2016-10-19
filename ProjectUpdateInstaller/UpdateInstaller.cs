using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProjectUpdateInstaller
{
    class UpdateInstaller
    {
        private const byte minArgCount = 2;

        private static string targetProcessName;
        private static string extractedUpdatePath;
        private static string targetExecutableFile;
        private static string currentDirectory = Directory.GetCurrentDirectory();
        private static string currentExecutableName = Assembly.GetEntryAssembly().GetName().Name;

        static void Main(string[] args)
        {
            if (args.Length < minArgCount)
            {
                throw new ArgumentException($"At least {minArgCount} arguments are required to launch {currentExecutableName}");
            }

            extractedUpdatePath = args[0];
            targetExecutableFile = args[1]; // should point to the executable to launch if the update is applied correctly. Eg.: Yal.exe
            targetProcessName = Path.GetFileNameWithoutExtension(targetExecutableFile);
            
            string message = null;
            var itemsFailedToUpdate = ApplyUpdate();

            if (itemsFailedToUpdate.Count != 0)
            {
                message = $"The following files/folders couldn't be updated:\n{string.Join("\n", itemsFailedToUpdate)}\nTry to update them manually";
            }
            else
            {
                // starts the newly updated 'targetExecutableFile' (Yal.exe)
                message = "Update successful";
                Process.Start(targetExecutableFile);
            }

            MessageBox.Show(message, currentExecutableName);
        }

        private static List<string> ApplyUpdate()
        {
            var failedToUpdate = new List<string>();

            // kill the <AssemblyName>.vshost process. Necessary when debugging through VS
            KillProcessByName(string.Concat(targetProcessName, ".vshost"));

            foreach (var item in Directory.GetFileSystemEntries(extractedUpdatePath, "*", SearchOption.AllDirectories))
            {
                var relativeItem = item.Replace(string.Concat(extractedUpdatePath, Path.DirectorySeparatorChar), "");

                try
                {
                    if (Directory.Exists(item))
                    {
                        Directory.CreateDirectory(relativeItem); // this doesn't fail if the dir is already in place
                    }
                    else // it's a file
                    {
                        if (File.Exists(relativeItem))
                        {
                            File.Delete(relativeItem);
                        }
                        File.Copy(item, relativeItem);
                    }
                }
                catch
                {
                    failedToUpdate.Add(relativeItem);
                }
            }

            return failedToUpdate;
        }

        private static void KillProcessByName(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
