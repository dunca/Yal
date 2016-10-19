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
                try
                {
                    client.Headers.Add(userAgent);
                    return client.DownloadString(githubProjectUrl);
                }
                catch (WebException ex)
                {
                    // url down/not internet connection/etc
                }
                return null;
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
            try
            {
                var serializer = new JavaScriptSerializer();
                dynamic parsedReleases = serializer.DeserializeObject(releasesJson);
                return parsedReleases[0];
            }
            catch
            {
                // string can't be parsed
            }

            return null;
        }

        public void InstallNewUpdate()
        {
            string downloadedFileName =  null;

            if (latestParsedReleaseData != null && !DownloadRelease(GetLatestReleaseUrl(latestParsedReleaseData), out downloadedFileName))
            {
                MessageBox.Show("Something went wrong while downloading the update", $"{currentAssemblyName}");
                return;
            }

            latestParsedReleaseData = null;

            Application.Exit();
            Process.Start(updateInstaller, $"{downloadedFileName} {Application.ExecutablePath}");
        }

        private bool DownloadRelease(string releaseUrl, out string downloadedFileName)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add(userAgent);

                downloadedFileName = Path.GetFileName(releaseUrl);

                try
                {
                    client.DownloadFile(releaseUrl, downloadedFileName);
                    return true;
                }
                catch (WebException ex)
                {
                    // no internet connection / dead url /etc
                }
                return false;
            }
        }

        private static int VersionStringToNumber(string applicationVersion)
        {
            return Convert.ToInt32(string.Join("", applicationVersion.Where(character => char.IsDigit(character))));
        }

        public bool CheckNewUpdate()
        {
            var latestReleasesJson = FetchReleasesList();

            if (latestReleasesJson != null)
            {
                latestParsedReleaseData = ParsedLastReleaseData(latestReleasesJson);
                
                if (latestParsedReleaseData != null && GetLatestReleaseVersion(latestParsedReleaseData) > currentApplicationVersion)
                {
                    return true;
                }
            }
            MessageBox.Show("Something went wrong when checking for updates", $"{currentAssemblyName}");
            return false;
        }
    }
}
