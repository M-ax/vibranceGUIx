# Building and packaging vibranceGUIx

From the repository root on Windows:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
```

Requires Windows PowerShell 5.1+, Visual Studio MSBuild with the .NET desktop
build tools (including the Windows SDK resource generator), and Internet access
on the first build. The .NET SDK alone is not enough for this legacy project.
To select MSBuild explicitly:

```powershell
.\build.ps1 -MSBuildPath '<path-to-MSBuild.exe>'
```

`-SkipInstaller` builds only the application payload. Run only one build at a time
in a checkout. Downloads are confined to `.tools/`; extracted dependencies are in
`packages/`. Nothing is installed globally and no installer is run by the build.
The build verifies pinned SHA256 hashes before extracting packages. The compiler
is the unofficial NuGet redistribution `Tools.InnoSetup` 6.7.3; review its bundled
license before commercial use. Framework reference assemblies are 1.0.3 and
CommonServiceLocator is 1.3 (NuGet's normalized URL version is 1.3.0).

The application still targets .NET Framework 4.0 and x86. Modern MSBuild uses
the pinned framework reference assemblies rather than requiring the old targeting
pack. Obsolete Fody/Costura weaving has been removed: the managed dependency is
distributed beside the executable instead. **Do not distribute the executable
alone.** The NVIDIA native library stays embedded with its original resource name
and existing runtime extraction behavior.

## Outputs and reproducibility

- `artifacts/payload/`: executable, `.exe.config`, CommonServiceLocator DLL, quick-start guide and dependency notices.
- `artifacts/vibranceGUIx-2.3.1.3-Setup.exe`: installer (name follows executable file version).
- `artifacts/release/vibranceGUIx-Setup.exe`: identical installer with a stable download filename.
- `artifacts/release/vibranceGUIx-Portable.zip`: the complete portable payload.
- `artifacts/release/SHA256SUMS.txt`: SHA256 hashes of the installer and portable ZIP.
- `artifacts/build.log`, `artifacts/build.binlog`: detailed MSBuild evidence.
- `artifacts/payload-sha256.json`: payload hashes.

The version is read from the built executable, not duplicated in the installer.
Keep the installer AppId stable across releases. Increment the application file
version when releasing a new version. The installer source remains
`installer/vibranceGUI.iss`; the executable remains `vibrance.GUI.exe`.

Pinned downloads, a clean Release/x86 rebuild, deterministic C# compilation and
source-path mapping make builds repeatable with the same VS compiler and inputs.
This does not promise byte-identical installers across times or compiler versions;
Inno Setup can include timestamps. The installer is unsigned, so Windows may show
an unknown-publisher/SmartScreen warning. Code signing is a separate release step.

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\installer\verify-payload.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\installer\verify-release.ps1
```

The payload check verifies the PE architecture and that the embedded NVIDIA DLL
is byte-for-byte identical to the source DLL without starting the app or changing
GPU settings. The release check verifies both checksums, installer identity, ZIP
inventory and the hashes of all archived payload files.

## Publishing a release

Build from the exact commit that will be tagged. Run both verifiers and the
regression tests before uploading. Create a draft GitHub release targeting that
commit, upload the three files from `artifacts/release`, verify the uploaded
digests, then publish the draft. Do not overwrite an existing published version.

Keep these asset filenames stable so the README's `releases/latest/download`
links continue to work. Put the version in the release tag/title and executable
metadata. Use [release notes](RELEASE-NOTES.md) as the release description.
After publication, download the public assets and verify their SHA256 hashes.
The build script itself never publishes, signs, or installs anything.

## Installer behavior

Windows 10 or later and .NET Framework 4.8+ are required by the installer. It
checks the framework release registry value; it never downloads/installs runtimes.
The original x86 application requires working vendor GPU drivers.

Fresh installation is per-user at `%LocalAppData%\Programs\vibranceGUIx`, without
UAC elevation; upgrades retain the existing directory. Setup creates a Start Menu
shortcut and a registered uninstaller. Desktop
shortcut, launch after setup, and **Launch on boot (at sign-in)** are optional.
Launch after setup is selected by default; launch at sign-in remains opt-in.
The startup checkbox defaults off on fresh installs and follows the live Run
value on upgrades, including changes made inside the application.

Startup uses `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`, value
`vibranceGUI`, data `"full\path\vibrance.GUI.exe" -minimized`. Selecting the task
updates that value to the new path. Deselecting it removes the existing application
registration, including a portable copy, only if it has not changed since Setup
started. Uninstall removes it only if it exactly matches this installation's
command (case-insensitive), preserving another installation's registration.
Settings in `%AppData%\vibranceGUI` and the app's extracted native-library cache
are not removed on uninstall. The legacy Run value and settings path are retained
for compatibility.

The app's startup checkbox is checked only when the registered command matches
the running copy. After moving a portable copy, check it again to explicitly
update the path; another installation's registration is not taken over on launch.

Rebranded upgrades remove old `vibranceGUI` Start Menu/desktop shortcuts only when
their target is this installation's executable. Shortcuts to other copies are
left untouched.

Restart Manager is enabled for payload files; the application's named mutex
`vibranceGUI~Mutex` additionally prevents install/uninstall while the app is
running. Exit vibranceGUIx from its tray menu before upgrading. Setup does not
force-kill it or automatically restart it.

## Manual acceptance checklist (requires permission or a disposable VM)

The build and resource inspection do not prove UI/install behavior. Test:

1. Fresh standard-user install: no elevation, correct destination, Start Menu,
   optional desktop icon, startup unchecked, uninstall entry.
2. Select startup: verify exact quoted command above; sign out/in and confirm
   tray/minimized behavior.
3. Toggle startup inside the app, exit, upgrade: checkbox reflects the new choice.
   Confirm an existing installation keeps its directory after the branding change.
   Repeat with a path containing spaces and with startup disabled.
   Migrate a portable startup entry and deselect startup: verify the old entry is
   removed. Change it externally while Setup is open: verify deselection preserves
   the newly changed entry.
   Move a portable payload: its startup checkbox should be unchecked with a tooltip
   explaining the other registration; checking it should update the command.
   Upgrade a pre-rebrand installer: old shortcuts targeting this installation
   should be replaced by `vibranceGUIx` shortcuts; shortcuts to other copies remain.
4. Keep the app open during upgrade/uninstall: setup must request it be closed.
5. Uninstall: owned startup entry and shortcuts removed, settings retained.
   Replace the startup value with another path first and confirm it is preserved.
6. Missing .NET 4.8: clear prerequisite message, no installation.
7. Launch installed payload on supported NVIDIA hardware; confirm native load and
   vibrance changes. Repeat for AMD. Check launch-at-sign-in after reboot.

The installer wizard was opened with permission during earlier validation, but
installation completion was not confirmed. Installation, uninstallation and
sign-in behavior remain unverified; see the [verification notes](VERIFICATION.md).
