using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using vibrance.GUI.NVIDIA;

namespace vibrance.GUI.common
{

    class SettingsController : ISettingsController
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetPrivateProfileString(
           string lpAppName,
           string lpKeyName,
           string lpDefault,
           StringBuilder lpReturnedString,
           uint nSize,
           string lpFileName);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WritePrivateProfileString(string lpAppName,
          string lpKeyName, string lpString, string lpFileName);

        const string SzSectionName = "Settings";
        const string SzKeyNameInactive = "inactiveValue";
        const string SzKeyNameRefreshRate = "refreshRate";
        const string SzKeyNameAffectPrimaryMonitorOnly = "affectPrimaryMonitorOnly";
        const string SzKeyNameNeverSwitchResolution = "neverSwitchResolution";
        const string SzKeyNameGraphicsAdapter = "graphicsAdapter";

        private string _fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "\\vibranceGUI\\vibranceGUI.ini";
        private string _fileNameApplicationSettings = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "\\vibranceGUI\\applicationData.xml";

        public SettingsController() { }

        public SettingsController(GraphicsAdapter adapter)
        {
            if (adapter != GraphicsAdapter.Unknown)
            {
                string directory = Path.GetDirectoryName(_fileName);
                _fileName = Path.Combine(directory, "vibranceGUI." + adapter + ".ini");
                _fileNameApplicationSettings = Path.Combine(directory, "applicationData." + adapter + ".xml");
            }
        }

        // The old settings had no vendor tag. Never guess which GPU they belong to:
        // AMD saturation and NVIDIA vibrance have different scales.
        public bool CanImportLegacyProfile(GraphicsAdapter adapter)
        {
            if (adapter == GraphicsAdapter.Unknown || File.Exists(_fileName))
                return false;
            string directory = Path.GetDirectoryName(_fileName);
            string legacyIni = Path.Combine(directory, "vibranceGUI.ini");
            string legacyApplications = Path.Combine(directory, "applicationData.xml");
            if (!File.Exists(legacyIni) || !File.Exists(legacyApplications))
                return false;

            StringBuilder owner = new StringBuilder(32);
            GetPrivateProfileString(SzSectionName, "legacyProfileAdapter", "", owner, (uint)owner.Capacity, legacyIni);
            return owner.Length == 0 || owner.ToString() == adapter.ToString();
        }

        public void ImportLegacyProfile(GraphicsAdapter adapter)
        {
            if (!CanImportLegacyProfile(adapter))
                return;
            string directory = Path.GetDirectoryName(_fileName);
            string legacyIni = Path.Combine(directory, "vibranceGUI.ini");
            string legacyApplications = Path.Combine(directory, "applicationData.xml");
            File.Copy(legacyApplications, _fileNameApplicationSettings, true);
            File.Copy(legacyIni, _fileName, false);
            WritePrivateProfileString(SzSectionName, "legacyProfileAdapter", adapter.ToString(), legacyIni);
        }

        public GraphicsAdapterPreference ReadGraphicsAdapterPreference()
        {
            StringBuilder value = new StringBuilder(32);
            GetPrivateProfileString(SzSectionName, SzKeyNameGraphicsAdapter, "Auto", value,
                (uint)value.Capacity, _fileName);
            GraphicsAdapterPreference preference;
            return Enum.TryParse(value.ToString(), true, out preference) &&
                Enum.IsDefined(typeof(GraphicsAdapterPreference), preference)
                ? preference : GraphicsAdapterPreference.Auto;
        }

        public bool SetGraphicsAdapterPreference(GraphicsAdapterPreference preference)
        {
            return Enum.IsDefined(typeof(GraphicsAdapterPreference), preference) &&
                SetVibranceSetting(SzKeyNameGraphicsAdapter, preference.ToString());
        }

        public bool SetVibranceSettings(string windowsLevel, string affectPrimaryMonitorOnly, string neverSwitchResolution, List<ApplicationSetting> applicationSettings)
        {
            if (!PrepareFile())
            {
                return false;
            }

            if (!WritePrivateProfileString(SzSectionName, SzKeyNameInactive, windowsLevel, _fileName) ||
                !WritePrivateProfileString(SzSectionName, SzKeyNameAffectPrimaryMonitorOnly, affectPrimaryMonitorOnly, _fileName) ||
                !WritePrivateProfileString(SzSectionName, SzKeyNameNeverSwitchResolution, neverSwitchResolution, _fileName))
                return false;

            try
            {
                using (var writer = System.Xml.XmlWriter.Create(_fileNameApplicationSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<ApplicationSetting>));
                    serializer.Serialize(writer, applicationSettings);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SetVibranceSetting(string szKeyName, string value)
        {
            if (!PrepareFile())
            {
                return false;
            }

            return WritePrivateProfileString(SzSectionName, szKeyName, value, _fileName);
        }

        private bool PrepareFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_fileName));
                if (!IsFileExisting(_fileName))
                {
                    using (File.Create(_fileName)) { }
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public void ReadVibranceSettings(GraphicsAdapter graphicsAdapter, out int vibranceWindowsLevel, out bool affectPrimaryMonitorOnly, out bool neverSwitchResolution, out List<ApplicationSetting> applicationSettings)
        {
            int defaultLevel = 0; 
            int maxLevel = 0;
            if (graphicsAdapter == GraphicsAdapter.Nvidia)
            {
                defaultLevel = NvidiaDynamicVibranceProxy.NvapiDefaultLevel;
                maxLevel = NvidiaDynamicVibranceProxy.NvapiMaxLevel;
            }
            if (graphicsAdapter == GraphicsAdapter.Amd)
            {
                // todo
                defaultLevel = 100;
                maxLevel = 300;
            }
            int minLevel = 0;

            if (!IsFileExisting(_fileName) || !IsFileExisting(_fileNameApplicationSettings))
            {
                vibranceWindowsLevel = defaultLevel;
                affectPrimaryMonitorOnly = false;
                applicationSettings = new List<ApplicationSetting>();
                neverSwitchResolution = false;
                return;
            }

            string szDefault = "";

            StringBuilder szValueInactive = new StringBuilder(1024);
            GetPrivateProfileString(SzSectionName,
                SzKeyNameInactive,
                szDefault,
                szValueInactive,
                Convert.ToUInt32(szValueInactive.Capacity),
                _fileName);

            StringBuilder szValueRefreshRate = new StringBuilder(1024);
            GetPrivateProfileString(SzSectionName,
                SzKeyNameRefreshRate,
                szDefault,
                szValueRefreshRate,
                Convert.ToUInt32(szValueRefreshRate.Capacity),
                _fileName);

            StringBuilder szValueAffectPrimaryMonitorOnly = new StringBuilder(1024);
            GetPrivateProfileString(SzSectionName,
                SzKeyNameAffectPrimaryMonitorOnly,
                "false",
                szValueAffectPrimaryMonitorOnly,
                Convert.ToUInt32(szValueAffectPrimaryMonitorOnly.Capacity),
                _fileName);

            StringBuilder szValueNeverSwitchResolution = new StringBuilder(1024);
            GetPrivateProfileString(SzSectionName,
                SzKeyNameNeverSwitchResolution,
                "false",
                szValueNeverSwitchResolution,
                Convert.ToUInt32(szValueNeverSwitchResolution.Capacity),
                _fileName);

            try
            {
                vibranceWindowsLevel = int.Parse(szValueInactive.ToString());
                affectPrimaryMonitorOnly = bool.Parse(szValueAffectPrimaryMonitorOnly.ToString());
                neverSwitchResolution = bool.Parse(szValueNeverSwitchResolution.ToString());
            }
            catch (Exception)
            {
                vibranceWindowsLevel = defaultLevel;
                affectPrimaryMonitorOnly = false;
                applicationSettings = new List<ApplicationSetting>();
                neverSwitchResolution = false;
                return;
            }

            if (vibranceWindowsLevel < minLevel || vibranceWindowsLevel > maxLevel)
                vibranceWindowsLevel = defaultLevel;

            try
            {
                using (var reader = System.Xml.XmlReader.Create(_fileNameApplicationSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<ApplicationSetting>));
                    applicationSettings = (List<ApplicationSetting>)serializer.Deserialize(reader);
                }
                foreach (var setting in applicationSettings)
                {
                    if (setting.IngameLevel < minLevel || setting.IngameLevel > maxLevel)
                        setting.IngameLevel = defaultLevel;
                }
            }
            catch (Exception)
            {
                applicationSettings = new List<ApplicationSetting>();
            }
        }

        private bool IsFileExisting(string szFilename)
        {
            return File.Exists(szFilename);
        }
    }
}
