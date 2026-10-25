using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vibrance.GUI.common;
using vibrance.GUI.NVIDIA;

public static class RegressionTests
{
    private static int assertions;
    private static Assembly assembly;
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

    private static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        assertions++;
        Console.WriteLine("PASS: " + description);
    }

    private static object Settings(string directory, GraphicsAdapter adapter)
    {
        Type type = assembly.GetType("vibrance.GUI.common.SettingsController", true);
        object settings = Activator.CreateInstance(type, new object[] { adapter });
        string suffix = adapter == GraphicsAdapter.Unknown ? "" : "." + adapter;
        type.GetField("_fileName", PrivateInstance).SetValue(settings, Path.Combine(directory, "vibranceGUI" + suffix + ".ini"));
        type.GetField("_fileNameApplicationSettings", PrivateInstance).SetValue(settings, Path.Combine(directory, "applicationData" + suffix + ".xml"));
        return settings;
    }

    private static object Call(object target, string method, params object[] arguments)
    {
        return target.GetType().GetMethod(method).Invoke(target, arguments);
    }

    public static void Run(Assembly app, string fixtures, bool hardware, string nativePath, string screenshotPath)
    {
        assembly = app;
        Directory.CreateDirectory(fixtures);
        Check(((AssemblyProductAttribute)Attribute.GetCustomAttribute(app, typeof(AssemblyProductAttribute))).Product == "vibranceGUIx",
            "Assembly product is branded vibranceGUIx");
        foreach (bool nvidia in new[] { false, true })
        foreach (bool amd in new[] { false, true })
        foreach (GraphicsAdapterPreference preference in Enum.GetValues(typeof(GraphicsAdapterPreference)))
        {
            GraphicsAdapter expected = GraphicsAdapter.Unknown;
            if (preference == GraphicsAdapterPreference.Auto)
                expected = nvidia ? GraphicsAdapter.Nvidia : amd ? GraphicsAdapter.Amd : GraphicsAdapter.Unknown;
            if (preference == GraphicsAdapterPreference.Nvidia && nvidia) expected = GraphicsAdapter.Nvidia;
            if (preference == GraphicsAdapterPreference.Amd && amd) expected = GraphicsAdapter.Amd;
            Check(GraphicsAdapterHelper.SelectAdapter(preference, nvidia, amd) == expected,
                "Selection " + preference + " NVIDIA=" + nvidia + " AMD=" + amd);
        }

        object preferences = Settings(fixtures, GraphicsAdapter.Unknown);
        Check((GraphicsAdapterPreference)Call(preferences, "ReadGraphicsAdapterPreference") == GraphicsAdapterPreference.Auto, "Fresh install defaults to Auto");
        foreach (GraphicsAdapterPreference preference in Enum.GetValues(typeof(GraphicsAdapterPreference)))
        {
            Check((bool)Call(preferences, "SetGraphicsAdapterPreference", preference), "Save " + preference);
            Check((GraphicsAdapterPreference)Call(Settings(fixtures, GraphicsAdapter.Unknown), "ReadGraphicsAdapterPreference") == preference, "Reload " + preference);
        }
        Call(preferences, "SetVibranceSetting", "graphicsAdapter", "999");
        Check((GraphicsAdapterPreference)Call(preferences, "ReadGraphicsAdapterPreference") == GraphicsAdapterPreference.Auto, "Invalid preference recovers to Auto");

        var applications = new List<ApplicationSetting> { new ApplicationSetting("Test", "test.exe", 250, null, false) };
        Check((bool)Call(preferences, "SetVibranceSettings", "25", "False", "True", applications), "Save legacy profile");
        object nvidiaSettings = Settings(fixtures, GraphicsAdapter.Nvidia);
        Call(nvidiaSettings, "ImportLegacyProfile", GraphicsAdapter.Nvidia);
        object[] values = { GraphicsAdapter.Nvidia, 0, false, false, null };
        Call(nvidiaSettings, "ReadVibranceSettings", values);
        Check((int)values[1] == 25 && (bool)values[3], "Import legacy desktop settings");
        Check(((List<ApplicationSetting>)values[4])[0].IngameLevel == 0, "Out-of-range NVIDIA game level resets safely");
        object amdSettings = Settings(fixtures, GraphicsAdapter.Amd);
        Call(amdSettings, "ImportLegacyProfile", GraphicsAdapter.Amd);
        Check(!File.Exists(Path.Combine(fixtures, "applicationData.Amd.xml")), "Legacy settings are not imported into both vendors");
        Check((bool)Call(amdSettings, "SetVibranceSettings", "50", "False", "False", applications), "Save independent AMD profile");
        values = new object[] { GraphicsAdapter.Amd, 0, false, false, null };
        Call(amdSettings, "ReadVibranceSettings", values);
        Check((int)values[1] == 50 && ((List<ApplicationSetting>)values[4])[0].IngameLevel == 250, "AMD values below neutral and above NVIDIA range survive");
        values = new object[] { GraphicsAdapter.Nvidia, 0, false, false, null };
        Call(nvidiaSettings, "ReadVibranceSettings", values);
        Check((int)values[1] == 25, "Switching vendors preserves the NVIDIA profile");

        Application.EnableVisualStyles();
        using (var form = new VibranceGUI((x, y) => null, 0, 0, 63, 0, x => x.ToString(),
            GraphicsAdapter.Unknown, GraphicsAdapterPreference.Auto))
        {
            form.Show();
            DateTime deadline = DateTime.UtcNow.AddSeconds(2);
            while (DateTime.UtcNow < deadline) { Application.DoEvents(); System.Threading.Thread.Sleep(10); }
            var startup = (CheckBox)typeof(VibranceGUI).GetField("checkBoxAutostart", PrivateInstance).GetValue(form);
            var selector = (ComboBox)typeof(VibranceGUI).GetField("comboBoxGraphicsAdapter", PrivateInstance).GetValue(form);
            var slider = (TrackBar)typeof(VibranceGUI).GetField("trackBarWindowsLevel", PrivateInstance).GetValue(form);
            Check(form.Text == "vibranceGUIx", "Window title is branded vibranceGUIx");
            Check((string)typeof(VibranceGUI).GetField("StartupRegistrationName", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue() == "vibranceGUI",
                "Rebrand preserves the existing startup registration");
            Check(startup.Visible && startup.Enabled && startup.Text.Contains("Launch on boot"), "Launch on boot remains visible without a working GPU");
            Check(selector.Visible && selector.Enabled && selector.Items.Count == 3, "GPU override remains available for recovery");
            Check(!slider.Enabled && !form.IsGpuInitialized, "Unavailable GPU disables vibrance controls");
            if (!string.IsNullOrEmpty(screenshotPath))
            {
                using (var bitmap = new Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(bitmap, form.ClientRectangle);
                    bitmap.Save(screenshotPath);
                }
            }
            form.Close();
        }

        if (hardware)
        {
            GraphicsAdapter detected = GraphicsAdapterHelper.GetAdapter(GraphicsAdapterPreference.Auto);
            Console.WriteLine("Detected backend: " + detected);
            Check(detected == GraphicsAdapter.Nvidia, "This hybrid machine automatically selects NVIDIA");
            Check(GraphicsAdapterHelper.GetAdapter(GraphicsAdapterPreference.Amd) == GraphicsAdapter.Amd, "AMD override selects the installed Radeon driver");
            Check(LoadLibrary(nativePath) != IntPtr.Zero, "Load x86 NVIDIA bridge");
            Type proxyType = assembly.GetType("vibrance.GUI.NVIDIA.NvidiaDynamicVibranceProxy", true);
            var proxy = (IVibranceProxy)Activator.CreateInstance(proxyType, new object[] {
                new List<ApplicationSetting>(), new Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>>() });
            try
            {
                Check(proxy.GetVibranceInfo().isInitialized, "NVIDIA backend initializes with Digital Vibrance support");
                Console.WriteLine("GPU: " + proxy.GetVibranceInfo().szGpuName);
                Console.WriteLine("Controllable displays: " + proxy.GetVibranceInfo().displayHandles.Count);
            }
            finally
            {
                // Do not call HandleDvcExit: this read-only probe must not change colors.
                if (proxy.GetVibranceInfo().isInitialized) proxy.UnloadLibraryEx();
            }
        }
        Console.WriteLine("Passed " + assertions + " assertions. Fixtures: " + fixtures);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string path);
}
