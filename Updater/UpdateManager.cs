using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Reflection;

namespace Updater
{
    public class UpdateManager
    {
        private dynamic latestParsedReleaseData;
        private string currentApplicatioName;
        private int currentApplicationVersion;
        private Form currentApplicationInstance;

        private const string updateInstaller = "ProjectUpdateInstaller.exe";
        private string githubProjectUrl = "https://api.github.com/repos/sidf/{0}/releases";
        private string currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private const string userAgent = "User-Agent:Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";

        public UpdateManager(Form currentApplicationInstance)
        {
            this.currentApplicationInstance = currentApplicationInstance;
            currentApplicatioName = currentApplicationInstance.ProductName;
            githubProjectUrl = string.Format(githubProjectUrl, currentApplicatioName);
            currentApplicationVersion = VersionStringToNumber(currentApplicationInstance.ProductVersion);
        }

        private string FetchReleasesList()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add(userAgent);
                return client.DownloadString(githubProjectUrl);
            }
        }

        private int GetLatestReleaseVersion(dynamic parsedLastReleaseData)
        {
            return VersionStringToNumber(parsedLastReleaseData["name"]);
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

        public void InstallNewUpdate()
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

        public bool CheckNewUpdate()
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
                if (GetLatestReleaseVersion(latestParsedReleaseData) > currentApplicationVersion)
                {
                    return true;
                }
                return false;
            }

            MessageBox.Show($"Something went wrong when checking for updates: {errorMessage}", currentAssemblyName);
            return false;
        }
    }
}
