using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO.Compression;

namespace ProjectUpdateManager
{
    public class UpdateManager
    {
        private string currentApplicationName;
        private int currentApplicationVersion;
        private Dictionary<string, object> latestParsedReleaseData;

        private const string ignorableUpdateSuffix = "feature_testing.zip";
        private const string updateInstaller = "ProjectUpdateInstaller.exe";
        private string githubProjectUrl = "https://api.github.com/repos/sidf/{0}/releases";
        private string currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private const string userAgent = "User-Agent:Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";

        public UpdateManager()
        {
            currentApplicationName = Application.ProductName;
            githubProjectUrl = string.Format(githubProjectUrl, currentApplicationName);
            currentApplicationVersion = VersionStringToNumber(Application.ProductVersion);
        }

        private string FetchReleasesList()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add(userAgent);
                return client.DownloadString(githubProjectUrl);
            }
        }

        private string GetLatestReleaseVersion(dynamic parsedLastReleaseData)
        {
            return parsedLastReleaseData["name"];
        }

        private string GetLatestReleaseUrl(dynamic parsedLastReleaseData)
        {
            return parsedLastReleaseData["assets"][0]["browser_download_url"];
        }

        private Dictionary<string, object> ParsedLastReleaseData(string releasesJson)
        {
            var serializer = new JavaScriptSerializer();
            dynamic parsedReleases = serializer.DeserializeObject(releasesJson);
            return parsedReleases[0];
        }

        private string ExtractUpdate(string updatePath)
        {
            var extractionPath = Path.Combine(Path.GetTempPath(), string.Concat(updatePath, "_", Path.GetRandomFileName()));
            ZipFile.ExtractToDirectory(updatePath, extractionPath);
            return extractionPath;
        }

        private void InstallNewUpdate()
        {
            string extractedUpdatePath = null;
            string downloadedFileName =  null;

            try
            {
                downloadedFileName = DownloadRelease(GetLatestReleaseUrl(latestParsedReleaseData));
                extractedUpdatePath = ExtractUpdate(downloadedFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong while processing the update: {ex.Message}", currentAssemblyName);
                return;
            }

            File.Delete(downloadedFileName);

            // eg.: C:\Users\<User>\AppData\Local\Temp\Yal.v1.0.0.4.zip_2a3ewaiv.t4n\*
            var extractedUpdateRoot = Directory.GetDirectories(extractedUpdatePath);

            if (extractedUpdateRoot.Length == 1)
            {
                // eg.: C:\Users\<User>\AppData\Local\Temp\Yal.v1.0.0.4.zip_2a3ewaiv.t4n\Yal.v1.0.0.4\*
                extractedUpdatePath = Path.Combine(extractedUpdatePath, extractedUpdateRoot[0]);
            }

            Application.Exit();

            var process = new Process();
            process.StartInfo.FileName = Path.Combine(extractedUpdatePath, updateInstaller);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = $"{extractedUpdatePath} {Application.ExecutablePath}";

            process.Start();
        }

        private string DownloadRelease(string releaseUrl)
        {
           var downloadedFileName = Path.GetFileName(releaseUrl);

            using (var client = new WebClient())
            {
                client.Headers.Add(userAgent);
                client.DownloadFile(releaseUrl, downloadedFileName);
            }

            return downloadedFileName;
        }

        private static int VersionStringToNumber(string applicationVersion)
        {
            return Convert.ToInt32(string.Join("", applicationVersion.Where(character => char.IsDigit(character))));
        }

        public async void CheckNewUpdate(bool onlyMessageOnAvailableUpdate = false)
        {
            await Task.Run(() => InternalCheckNewUpdate(onlyMessageOnAvailableUpdate));
        }

        private void InternalCheckNewUpdate(bool onlyMessageOnAvailableUpdate)
        {
            string latestReleasesJson;
            string errorMessage = null;

            try
            {
                latestReleasesJson = FetchReleasesList();
                latestParsedReleaseData = ParsedLastReleaseData(latestReleasesJson);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
                
            if (errorMessage == null)
            {
                var rawLatestVersion = GetLatestReleaseVersion(latestParsedReleaseData);
                var numericLatestVersion = VersionStringToNumber(rawLatestVersion);

                if (!rawLatestVersion.EndsWith(ignorableUpdateSuffix) && numericLatestVersion > currentApplicationVersion)
                {
                    if (MessageBox.Show($"{currentApplicationName} {rawLatestVersion} is available. Would you like to automatically apply the update? The application will try restarting itself if everything goes right", 
                                        currentAssemblyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        InstallNewUpdate();
                    }
                }
                else if (!onlyMessageOnAvailableUpdate)
                {
                    MessageBox.Show($"You are already using the latest version of {currentApplicationName}", currentAssemblyName);
                }

                return;
            }

            if (!onlyMessageOnAvailableUpdate)
            {
                MessageBox.Show($"Something went wrong when checking for updates: {errorMessage}", currentAssemblyName);
            }
        }
    }
}
