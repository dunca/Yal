using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace Updater
{
    public class ProjectUpdater
    {
        private string currentApplicatioName;
        private int currentApplicationVersion;
        private string githubProjectUrl = "https://api.github.com/repos/sidf/{0}/releases";

        public ProjectUpdater(Form currentApplicationInstance)
        {
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
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                    var releasesJson = client.DownloadString(githubProjectUrl);
                    return releasesJson;
                }
                catch (WebException ex)
                {
                    // url down/not internet connection/etc
                }
                return null;
            }
        }

        private string GetLatestReleaseUrl(string releasesJson)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                dynamic parsedReleases = serializer.DeserializeObject(releasesJson);
                dynamic lastestRelease = parsedReleases[0];
                return lastestRelease["assets"][0]["browser_download_url"];
            }
            catch
            {
                // string can't be parsed
            }

            return null;
        }

        public bool DownloadLatestRelease()
        {
            var latestReleaseUrl = GetLatestReleaseUrl(FetchReleasesList());
            
            if (DownloadRelease(latestReleaseUrl))
            {
                MessageBox.Show($"Download complete, {latestReleaseUrl}");
                return true;
            }
            MessageBox.Show("Download not done");
            return false;
        }

        private bool DownloadRelease(string releaseUrl)
        {
            return true;
        }

        private static int VersionStringToNumber(string applicationVersion)
        {
            return Convert.ToInt32(string.Join("", applicationVersion.Where(character => char.IsDigit(character))));
        }
    }
}
