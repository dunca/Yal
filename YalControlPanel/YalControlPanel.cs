using System;
using System.Linq;
using System.Drawing;
using PluginInterfaces;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Utilities;

namespace YalControlPanel
{
    public class YalControlPanel : IPlugin
    {
        public string Name { get; } = "YalControlPanel";
        public string Version { get; } = "1.0";
        public string Description { get; } = "Access Windows control panel items using Yal";
        public bool RequiresActivator { get; } = false;
        public PluginItemSortingOption SortingOption { get; } = PluginItemSortingOption.ByNameLength;

        public Icon PluginIcon { get; }
        public string HelpText { get; } = @"The plugin's description is self explanatory.";
        public UserControl PluginUserControl
        {
            get
            {
                if (pluginUserControl == null || pluginUserControl.IsDisposed)
                {
                    pluginUserControl = new YalControlPanelUC();
                }
                return pluginUserControl;
            }
        }

        private YalControlPanelUC pluginUserControl;
        private Dictionary<string, string> ControlPanelItems { get; } = new Dictionary<string, string>();
        private string controlPath = string.Concat(Environment.GetEnvironmentVariable("SYSTEMROOT"), @"\system32\control.exe");
        private string osString = Utils.GetOsVersion();
        private string[] canonicalNames = @"Action Center;Microsoft.ActionCenter;7,8,8.1,10
Administrative Tools;Microsoft.AdministrativeTools;Vista,7,8,8.1,10
AutoPlay;Microsoft.AutoPlay;Vista,7,8,8.1,10
Biometric Devices;Microsoft.BiometricDevices;7,8,8.1,10
BitLocker Drive Encryption;Microsoft.BitLockerDriveEncryption;Vista,7,8,8.1,10
Color Management;Microsoft.ColorManagement;Vista,7,8,8.1,10
Credential Manager;Microsoft.CredentialManager;7,8,8.1,10
Date and Time;Microsoft.DateAndTime;Vista,7,8,8.1,10
Default Programs;Microsoft.DefaultPrograms;Vista,7,8,8.1,10
Device Manager;Microsoft.DeviceManager;Vista,7,8,8.1,10
Devices and Printers;Microsoft.DevicesAndPrinters;7,8,8.1,10
Display;Microsoft.Display;7,8,8.1,10
Ease of Access Center;Microsoft.EaseOfAccessCenter;Vista,7,8,8.1,10
Family Safety;Microsoft.ParentalControls;Vista,7,8,8.1,10
File History;Microsoft.FileHistory;8,8.1,10
Folder Options;Microsoft.FolderOptions;Vista,7,8,8.1,10
Fonts;Microsoft.Fonts;Vista,7,8,8.1,10
HomeGroup;Microsoft.HomeGroup;7,8,8.1,10
Indexing Options;Microsoft.IndexingOptions;Vista,7,8,8.1,10
Infrared;Microsoft.Infrared;7,8,8.1,10
Internet Options;Microsoft.InternetOptions;Vista,7,8,8.1,10
iSCSI Initiator;Microsoft.iSCSIInitiator;Vista,7,8,8.1,10
iSNS Server;Microsoft.iSNSServer;Vista,7,8,8.1,10
Keyboard;Microsoft.Keyboard;Vista,7,8,8.1,10
Language;Microsoft.Language;8,8.1,10
Location Settings;Microsoft.LocationSettings;8,8.1,10
Mouse;Microsoft.Mouse;Vista,7,8,8.1,10
MPIOConfiguration;Microsoft.MPIOConfiguration;7,8,8.1,10
Network and Sharing Center;Microsoft.NetworkAndSharingCenter;Vista,7,8,8.1,10
Notification Area Icons;Microsoft.NotificationAreaIcons;7,8,8.1,10
Pen and Touch;Microsoft.PenAndTouch;7,8,8.1,10
Personalization;Microsoft.Personalization;Vista,7,8,8.1,10
Phone and Modem;Microsoft.PhoneAndModem;7,8,8.1,10
Power Options;Microsoft.PowerOptions;Vista,7,8,8.1,10
Programs and Features;Microsoft.ProgramsAndFeatures;Vista,7,8,8.1,10
Recovery;Microsoft.Recovery;7,8,8.1,10
Region;Microsoft.RegionAndLanguage;7,8,8.1,10
RemoteApp and Desktop Connections;Microsoft.RemoteAppAndDesktopConnections;7,8,8.1,10
Sound;Microsoft.Sound;7,8,8.1,10
Speech Recognition;Microsoft.SpeechRecognition;7,8,8.1,10
Storage Spaces;Microsoft.StorageSpaces;8,8.1,10
Sync Center;Microsoft.SyncCenter;Vista,7,8,8.1,10
System;Microsoft.System;Vista,7,8,8.1,10
Tablet PC Settings;Microsoft.TabletPCSettings;Vista,7,8,8.1,10
Taskbar and Navigation;Microsoft.Taskbar;8,8.1,10
Troubleshooting;Microsoft.Troubleshooting;7,8,8.1,10
TSAppInstall;Microsoft.TSAppInstall;7,8,8.1,10
User Accounts;Microsoft.UserAccounts;Vista,7,8,8.1,10
Windows Anytime Upgrade;Microsoft.WindowsAnytimeUpgrade;Vista,7,8,8.1,10
Windows Defender;Microsoft.WindowsDefender;Vista,7,8,8.1,10
Windows Firewall;Microsoft.WindowsFirewall;Vista,7,8,8.1,10
Windows Mobility Center;Microsoft.MobilityCenter;Vista,7,8,8.1,10
Windows To Go;Microsoft.PortableWorkspaceCreator;8,8.1,10
Windows Update;Microsoft.WindowsUpdate;Vista,7,8,8.1,10
Work Folders;Microsoft.WorkFolders;8.1,10
".Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        public YalControlPanel()
        {
            PluginIcon = Utils.GetPluginIcon(Name);

            foreach (var entry in canonicalNames)
            {
                var items = entry.Split(';');

                if (items[2].Split(',').Any(version => version == osString))
                {
                    // add the identifier and the canonical name
                    ControlPanelItems.Add(items[0], items[1]);
                }
            }
        }

        public void SaveSettings()
        {
            pluginUserControl.SaveSettings();
        }

        public string[] GetItems(string input, out string[] itemInfo)
        {
            itemInfo = null;
            return ControlPanelItems.Count > 0 ? ControlPanelItems.Keys.ToArray() : null;
        }

        public void HandleExecution(string name)
        {
            //string ret = GetMatchingActivator(input);
            if (name != null)
            {
                Process.Start(controlPath, $"/name {ControlPanelItems[name]}");
            }
        }
    }
}
