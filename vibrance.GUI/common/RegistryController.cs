using System;
using Microsoft.Win32;

namespace vibrance.GUI.common
{
    class RegistryController : IRegistryController
    {
        private const string RunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public bool RegisterProgram(string appName, string pathToExe)
        {
            try
            {
                using (RegistryKey startupKey = Registry.CurrentUser.CreateSubKey(RunKey))
                {
                    if (startupKey == null)
                        return false;
                    startupKey.SetValue(appName, pathToExe, RegistryValueKind.String);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UnregisterProgram(string appName)
        {
            try
            {
                using (RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(RunKey, true))
                {
                    if (startupKey != null)
                        startupKey.DeleteValue(appName, false);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsProgramRegistered(string appName)
        {
            try
            {
                using (RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(RunKey))
                {
                    return startupKey != null && startupKey.GetValue(appName) != null;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsStartupPathUnchanged(string appName, string pathToExe)
        {
            try
            {
                using (RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(RunKey))
                {
                    return startupKey != null && string.Equals(startupKey.GetValue(appName) as string,
                        pathToExe, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
