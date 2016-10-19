using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.Compression;
using System.Collections.Generic;

namespace ProjectUpdateInstaller
{
    class UpdateInstaller
    {
        private const byte minArgCount = 2;
        private static string targetProcessName;
        private static string targetExecutableFile;
        private static string currentDirectory = Directory.GetCurrentDirectory();
        private static string currentExecutableName = Assembly.GetEntryAssembly().GetName().Name;

        static void Main(string[] args)
        {
            if (args.Length < minArgCount)
            {
                throw new ArgumentException($"At least {minArgCount} are required to launch {currentExecutableName}");
            }

            var updatePath = args[0]; // a path to the downloaded zip file containing the new version of the application
            targetExecutableFile = args[1]; // should point to the executable to launch if the update is applied correctly. Eg.: Yal.exe
            targetProcessName = Path.GetFileNameWithoutExtension(targetExecutableFile);

            
            var extractionDirectory = ExtractFile(updatePath);

            if (extractionDirectory == null)
            {
                return;
            }

            string message = null;
            List<string> itemsFailedToUpdate;

            if (!ApplyUpdate(extractionDirectory, out itemsFailedToUpdate))
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

        private static bool ApplyUpdate(string extractedUpdatePath, out List<string> failedToUpdate)
        {
            failedToUpdate = new List<string>();

            // eg.: C:\Users\<User>\AppData\Local\Temp\Yal.v1.0.0.4.zip_2a3ewaiv.t4n\*
            var extractedUpdateRoot = Directory.GetDirectories(extractedUpdatePath);

            if (extractedUpdateRoot.Length == 1)
            {
                // eg.: C:\Users\<User>\AppData\Local\Temp\Yal.v1.0.0.4.zip_2a3ewaiv.t4n\Yal.v1.0.0.4\*
                extractedUpdatePath = Path.Combine(extractedUpdatePath, extractedUpdateRoot[0]);
            }

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

            return failedToUpdate.Count == 0;
        }

        private static void KillProcessByName(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }

        private static string ExtractFile(string updatePath)
        {
            var extractionPath = Path.Combine(Path.GetTempPath(), string.Concat(updatePath, "_", Path.GetRandomFileName()));

            try
            {
                ZipFile.ExtractToDirectory(updatePath, extractionPath);
                return extractionPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, currentExecutableName);
            }

            return null;
        }
    }
}
