using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using vibrance.GUI.AMD.vendor;

namespace vibrance.GUI.common
{
    public enum GraphicsAdapter
    {
        Unknown = 0,
        Nvidia = 1,
        Amd = 2
    }

    public enum GraphicsAdapterPreference
    {
        Auto = 0,
        Nvidia = 1,
        Amd = 2
    }

    public static class GraphicsAdapterHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr module);

        private const string _nvidiaDllName = "nvapi.dll";
        private static readonly string _amdDllName = Environment.Is64BitOperatingSystem 
            ? AMD.vendor.adl64.AdlImport.AtiadlFileName
            : AMD.vendor.adl32.AdlImport.AtiadlFileName;

        // Keep the selection policy separate from hardware probing so it is deterministic
        // and testable. An explicit choice must never silently select the other vendor.
        public static GraphicsAdapter SelectAdapter(GraphicsAdapterPreference preference, bool nvidiaAvailable, bool amdAvailable)
        {
            switch (preference)
            {
                case GraphicsAdapterPreference.Nvidia:
                    return nvidiaAvailable ? GraphicsAdapter.Nvidia : GraphicsAdapter.Unknown;
                case GraphicsAdapterPreference.Amd:
                    return amdAvailable ? GraphicsAdapter.Amd : GraphicsAdapter.Unknown;
                default:
                    return nvidiaAvailable ? GraphicsAdapter.Nvidia :
                        amdAvailable ? GraphicsAdapter.Amd : GraphicsAdapter.Unknown;
            }
        }

        public static GraphicsAdapter GetAdapter(GraphicsAdapterPreference preference)
        {
            bool nvidiaInstalled = false;
            bool amdInstalled = false;
            // Driver DLLs can remain after a GPU is removed. Only consider installed
            // hardware, including an iGPU with no monitor attached.
            using (var searcher = new ManagementObjectSearcher("SELECT PNPDeviceID FROM Win32_VideoController"))
            using (var controllers = searcher.Get())
            {
                foreach (ManagementObject controller in controllers)
                {
                    using (controller)
                    {
                        string deviceId = Convert.ToString(controller["PNPDeviceID"]);
                        nvidiaInstalled |= deviceId.IndexOf("VEN_10DE", StringComparison.OrdinalIgnoreCase) >= 0;
                        amdInstalled |= deviceId.IndexOf("VEN_1002", StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
            }

            bool nvidiaAvailable = nvidiaInstalled && IsAdapterAvailable(_nvidiaDllName);
            // Do not initialize ADL on hybrid systems when NVIDIA is the selected backend.
            if (preference != GraphicsAdapterPreference.Amd && nvidiaAvailable)
            {
                return GraphicsAdapter.Nvidia;
            }
            bool amdAvailable = false;
            if (preference != GraphicsAdapterPreference.Nvidia && amdInstalled && IsAdapterAvailable(_amdDllName))
            {
                using (IAmdAdapter amdAdapter = Environment.Is64BitOperatingSystem
                    ? (IAmdAdapter)new AmdAdapter64() : new AmdAdapter32())
                {
                    amdAvailable = amdAdapter.IsAvailable();
                }
            }
            return SelectAdapter(preference, nvidiaAvailable, amdAvailable);
        }

        private static bool IsAdapterAvailable(string dllName)
        {
            IntPtr module = IntPtr.Zero;
            try
            {
                string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
                module = LoadLibrary(Path.Combine(systemFolder, dllName));
                return module != IntPtr.Zero;
            }
            finally
            {
                if (module != IntPtr.Zero)
                    FreeLibrary(module);
            }
        }
    }
}
