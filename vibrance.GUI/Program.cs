using System.ComponentModel;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using vibrance.GUI.AMD;
using vibrance.GUI.AMD.vendor;
using vibrance.GUI.AMD.vendor.utils;
using vibrance.GUI.common;
using vibrance.GUI.NVIDIA;

namespace vibrance.GUI
{
    static class Program
    {
        private const string MessageBoxCaption = "vibranceGUIx Error";

        [STAThread]
        static void Main(string[] args)
        {
            bool result = false;
            Mutex mutex = new Mutex(true, "vibranceGUI~Mutex", out result);
            if (!result)
            {
                MessageBox.Show("Only one instance of vibranceGUIx or vibranceGUI can run at a time.", MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            NativeMethods.SetDllDirectory(CommonUtils.GetVibrance_GUI_AppDataPath());

            var preference = new SettingsController().ReadGraphicsAdapterPreference();
            GraphicsAdapter adapter = GraphicsAdapter.Unknown;
            string startupError = null;
            try
            {
                adapter = GraphicsAdapterHelper.GetAdapter(preference);
            }
            catch (Exception ex)
            {
                startupError = "GPU detection failed: " + ex.Message;
            }
            VibranceGUI vibranceGui = null;

            if (adapter == GraphicsAdapter.Amd)
            {
                Func<List<ApplicationSetting>, Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>>, IVibranceProxy> getProxy = (x, y) => new AmdDynamicVibranceProxy(Environment.Is64BitOperatingSystem
                    ? new AmdAdapter64()
                    : (IAmdAdapter)new AmdAdapter32(), x, y);
                vibranceGui = new VibranceGUI(getProxy, 
                    100, 
                    0,
                    300,
                    100,
                    x => x.ToString(), adapter, preference);
            }
            else if (adapter == GraphicsAdapter.Nvidia)
            {
                const string nvidiaAdapterName = "vibranceDLL.dll";
                string resourceName = $"{typeof(Program).Namespace}.NVIDIA.{nvidiaAdapterName}";
                try
                {
                    CommonUtils.LoadUnmanagedLibraryFromResource(
                        Assembly.GetExecutingAssembly(),
                        resourceName,
                        nvidiaAdapterName);
                    Marshal.PrelinkAll(typeof(NvidiaDynamicVibranceProxy));
                }
                catch (Exception ex)
                {
                    startupError = "NVIDIA driver initialization failed: " + ex.Message;
                }

                vibranceGui = new VibranceGUI(
                    (x, y) => startupError == null ? new NvidiaDynamicVibranceProxy(x, y) : null,
                    NvidiaDynamicVibranceProxy.NvapiDefaultLevel,
                    NvidiaDynamicVibranceProxy.NvapiDefaultLevel,
                    NvidiaDynamicVibranceProxy.NvapiMaxLevel,
                    NvidiaDynamicVibranceProxy.NvapiDefaultLevel,
                    x => NvidiaVibranceValueWrapper.Find(x).Percentage, adapter, preference);
            }
            else
            {
                startupError = startupError ?? "The selected GPU is unavailable. Choose Auto or another GPU, then restart. Check that its driver is installed.";
                vibranceGui = new VibranceGUI((x, y) => null, 0, 0, 63, 0,
                    x => NvidiaVibranceValueWrapper.Find(x).Percentage, adapter, preference);
            }
            if (startupError != null)
                vibranceGui.SetStartupError(startupError);
            if (args.Contains("-minimized") && vibranceGui.IsGpuInitialized)
            {
                vibranceGui.WindowState = FormWindowState.Minimized;
                vibranceGui.SetAllowVisible(false);
            }
            vibranceGui.Text += String.Format(" ({0}, {1})", adapter.ToString().ToUpper(), Application.ProductVersion);
            Application.Run(vibranceGui);

            GC.KeepAlive(mutex);
        }
    }
}
