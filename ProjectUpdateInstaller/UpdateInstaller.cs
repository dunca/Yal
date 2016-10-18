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

            var updatePath = args[0];
            targetExecutableFile = args[1];
            targetProcessName = Path.GetFileNameWithoutExtension(targetExecutableFile);

            List<string> itemsFailedToUpdate;
            var extractionDirectory = ExtractFile(updatePath);

            if (extractionDirectory == null)
            {
                MessageBox.Show("Something went wrong while extracting the new version. Try to update manually", currentExecutableName);
                
            }
            else if (!ApplyUpdate(extractionDirectory, out itemsFailedToUpdate))
            {
                MessageBox.Show($"The following files/folders couldn't be updated:\n{string.Join("\n", itemsFailedToUpdate)}\nTry to update them manually", 
                                currentExecutableName);
            }
            else
            {
                Process.Start(targetExecutableFile);
            }
        }

        private static bool ApplyUpdate(string extractedUpdatePath, out List<string> failedToUpdate)
        {
            // eg.: C:\Users\<User>\AppData\Local\Temp\Yal.v1.0.0.4.zip\2a3ewaiv.t4n\*
            var extractedUpdateDirs = Directory.GetDirectories(extractedUpdatePath);

            if (extractedUpdateDirs.Length == 1)
            {
                // eg.: C:\Users\<User>\AppData\Local\Temp\Yal.v1.0.0.4.zip\2a3ewaiv.t4n\Yal.v1.0.0.4\*
                extractedUpdatePath = Path.Combine(extractedUpdatePath, extractedUpdateDirs[0]);
            }

            KillProcessByName(targetProcessName);

            // kill the <AssemblyName>.vshost process (if it's running) otherwise we won't be able to replace it with the updated one
            KillProcessByName(string.Concat(targetProcessName, ".vshost"));

            var fsEntries = Directory.GetFileSystemEntries(extractedUpdatePath, "*", SearchOption.AllDirectories);

            failedToUpdate = new List<string>();

            foreach (var item in fsEntries)
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
                catch (Exception ex)
                {
                    failedToUpdate.Add(relativeItem);
                }
            }

            return failedToUpdate.Count == 0;
        }

        private static void KillProcessByName(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        private static string ExtractFile(string archiveFile)
        {
            var extractionDirectory = Path.Combine(Path.GetTempPath(), string.Concat(archiveFile, "_", Path.GetRandomFileName()));

            try
            {
                ZipFile.ExtractToDirectory(archiveFile, extractionDirectory);
                return extractionDirectory;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }
    }
}
