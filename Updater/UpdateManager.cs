using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace ProjectUpdateManager
{
    public class UpdateManager
    {
        private dynamic latestParsedReleaseData;
        private string currentApplicationName;
        private int currentApplicationVersion;
        private Form mainWindow;

        private const string updateInstaller = "ProjectUpdateInstaller.exe";
        private string githubProjectUrl = "https://api.github.com/repos/sidf/{0}/releases";
        private string currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private const string userAgent = "User-Agent:Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";

        public UpdateManager(Form mainWindow)
        {
            this.mainWindow = mainWindow;
            currentApplicationName = mainWindow.ProductName;
            githubProjectUrl = string.Format(githubProjectUrl, currentApplicationName);
            currentApplicationVersion = VersionStringToNumber(mainWindow.ProductVersion);
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

        private dynamic ParsedLastReleaseData(string releasesJson)
        {
            var serializer = new JavaScriptSerializer();
            dynamic parsedReleases = serializer.DeserializeObject(releasesJson);
            return parsedReleases[0];
        }

        private void InstallNewUpdate()
        {
            string downloadedFileName =  null;

            try
            {
                downloadedFileName = DownloadRelease(GetLatestReleaseUrl(latestParsedReleaseData));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong while downloading the update: {ex.Message}", currentAssemblyName);
                return;
            }

            Application.Exit();
            latestParsedReleaseData = null;

            var process = new Process();
            process.StartInfo.FileName = updateInstaller;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = $"{downloadedFileName} {Application.ExecutablePath}";

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

        public void CheckNewUpdate()
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

                if (numericLatestVersion > currentApplicationVersion)
                {
                    if (MessageBox.Show($"{currentApplicationName} {rawLatestVersion} is available. Would you like to automatically apply the update? The application will try restarting itself if everything goes right", 
                                        currentAssemblyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        InstallNewUpdate();
                    }
                }
                else
                {
                    MessageBox.Show($"You are already using the latest version of {currentApplicationName}", currentAssemblyName);
                }

                return;
            }

            MessageBox.Show($"Something went wrong when checking for updates: {errorMessage}", currentAssemblyName);
        }
    }
}
