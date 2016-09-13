using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterfaces;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Utilities;

namespace YalControlPanel
{
    public class YalControlPanel : IPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public Icon PluginIcon { get; }
        public List<string> Activators { get; }
        public bool FileLikeOutput { get; }

        private Dictionary<string, string> ControlPanelItems { get; }
        private YalControlPanelUC ControlPanelPluginInstance { get; set; }
        string controlPath = string.Concat(Environment.GetEnvironmentVariable("SYSTEMROOT"), @"\system32\control.exe");
        private string osString = Utils.GetOsVersion();
        private string[] canonicalNames = @"Action Center;Microsoft.ActionCenter;7,8,8.1
Administrative Tools;Microsoft.AdministrativeTools;Vista,7,8,8.1
AutoPlay;Microsoft.AutoPlay;Vista,7,8,8.1
Biometric Devices;Microsoft.BiometricDevices;7,8,8.1
BitLocker Drive Encryption;Microsoft.BitLockerDriveEncryption;Vista,7,8,8.1
Color Management;Microsoft.ColorManagement;Vista,7,8,8.1
Credential Manager;Microsoft.CredentialManager;7,8,8.1
Date and Time;Microsoft.DateAndTime;Vista,7,8,8.1
Default Programs;Microsoft.DefaultPrograms;Vista,7,8,8.1
Device Manager;Microsoft.DeviceManager;Vista,7,8,8.1
Devices and Printers;Microsoft.DevicesAndPrinters;7,8,8.1
Display;Microsoft.Display;7,8,8.1
Ease of Access Center;Microsoft.EaseOfAccessCenter;Vista,7,8,8.1
Family Safety;Microsoft.ParentalControls;Vista,7,8,8.1
File History;Microsoft.FileHistory;8,8.1
Folder Options;Microsoft.FolderOptions;Vista,7,8,8.1
Fonts;Microsoft.Fonts;Vista,7,8,8.1
HomeGroup;Microsoft.HomeGroup;7,8,8.1
Indexing Options;Microsoft.IndexingOptions;Vista,7,8,8.1
Infrared;Microsoft.Infrared;7,8,8.1
Internet Options;Microsoft.InternetOptions;Vista,7,8,8.1
iSCSI Initiator;Microsoft.iSCSIInitiator;Vista,7,8,8.1
iSNS Server;Microsoft.iSNSServer;Vista,7,8,8.1
Keyboard;Microsoft.Keyboard;Vista,7,8,8.1
Language;Microsoft.Language;8,8.1
Location Settings;Microsoft.LocationSettings;8,8.1
Mouse;Microsoft.Mouse;Vista,7,8,8.1
MPIOConfiguration;Microsoft.MPIOConfiguration;7,8,8.1
Network and Sharing Center;Microsoft.NetworkAndSharingCenter;Vista,7,8,8.1
Notification Area Icons;Microsoft.NotificationAreaIcons;7,8,8.1
Pen and Touch;Microsoft.PenAndTouch;7,8,8.1
Personalization;Microsoft.Personalization;Vista,7,8,8.1
Phone and Modem;Microsoft.PhoneAndModem;7,8,8.1
Power Options;Microsoft.PowerOptions;Vista,7,8,8.1
Programs and Features;Microsoft.ProgramsAndFeatures;Vista,7,8,8.1
Recovery;Microsoft.Recovery;7,8,8.1
Region;Microsoft.RegionAndLanguage;7,8,8.1
RemoteApp and Desktop Connections;Microsoft.RemoteAppAndDesktopConnections;7,8,8.1
Sound;Microsoft.Sound;7,8,8.1
Speech Recognition;Microsoft.SpeechRecognition;7,8,8.1
Storage Spaces;Microsoft.StorageSpaces;8,8.1
Sync Center;Microsoft.SyncCenter;Vista,7,8,8.1
System;Microsoft.System;Vista,7,8,8.1
Tablet PC Settings;Microsoft.TabletPCSettings;Vista,7,8,8.1
Taskbar and Navigation;Microsoft.Taskbar;8,8.1
Troubleshooting;Microsoft.Troubleshooting;7,8,8.1
TSAppInstall;Microsoft.TSAppInstall;7,8,8.1
User Accounts;Microsoft.UserAccounts;Vista,7,8,8.1
Windows Anytime Upgrade;Microsoft.WindowsAnytimeUpgrade;Vista,7,8,8.1
Windows Defender;Microsoft.WindowsDefender;Vista,7,8,8.1
Windows Firewall;Microsoft.WindowsFirewall;Vista,7,8,8.1
Windows Mobility Center;Microsoft.MobilityCenter;Vista,7,8,8.1
Windows To Go;Microsoft.PortableWorkspaceCreator;8,8.1
Windows Update;Microsoft.WindowsUpdate;Vista,7,8,8.1
Work Folders;Microsoft.WorkFolders;8.1
".Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        public YalControlPanel()
        {
            Name = "YalControlPanel";
            Version = "1.0";
            Description = "Access Windows control panel items using Yal";

            try
            {
                PluginIcon = Icon.ExtractAssociatedIcon(string.Concat(Directory.GetCurrentDirectory(), @"\plugins\icons\", Name, ".ico"));
            }
            catch
            {
                //MessageBox.Show((Directory.GetCurrentDirectory()));
            }
            FileLikeOutput = true;

            ControlPanelItems = new Dictionary<string, string>();

            foreach (var entry in canonicalNames)
            {
                var items = entry.Split(';');

                if (items[2].Split(',').Any(version => version == osString))
                {
                    // add the identifier and the canonical name
                    ControlPanelItems.Add(items[0], items[1]);
                }
            }

            Activators = new List<string>(ControlPanelItems.Keys);
        }

        public void SaveSettings()
        {
            ControlPanelPluginInstance.SaveSettings();
        }

        public UserControl GetUserControl()
        {
            if (ControlPanelPluginInstance == null || ControlPanelPluginInstance.IsDisposed)
            {
                ControlPanelPluginInstance = new YalControlPanelUC();
            }
            return ControlPanelPluginInstance;
        }

        public bool TryParseInput(string input, out string output)
        {
            output = "";
            string ret = GetMatchingActivator(input);
            if (ret == null)
            {
                return false;
            }
            else
            {
                output = ret;
                return true;
            }
        }

        public void HandleExecution(string input)
        {
            string ret = GetMatchingActivator(input);
            if (ret != null)
            {
                Process.Start(controlPath, $"/name {ControlPanelItems[ret]}");
            }
        }

        private string GetMatchingActivator(string input)
        {
            foreach (string activator in Activators)
            {
                if (activator.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                {
                    return activator;
                }
            }
            return null;
        }
    }
}
