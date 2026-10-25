# vibranceGUIx — Digital Vibrance for Windows 11 (2026)

**[Download Windows installer](https://github.com/M-ax/vibranceGUIx/releases/latest/download/vibranceGUIx-Setup.exe)** ·
[Portable ZIP](https://github.com/M-ax/vibranceGUIx/releases/latest/download/vibranceGUIx-Portable.zip) ·
[Release notes](https://github.com/M-ax/vibranceGUIx/releases/latest)

**Automate NVIDIA Digital Vibrance and AMD Radeon saturation with separate game
and desktop color settings.** vibranceGUIx is a Windows utility that uses graphics
driver APIs to apply a saved color level when a configured game is in focus and
restore your desktop level when you switch away.

This fork adds **NVIDIA-first selection for PCs with a dedicated GeForce GPU and
an AMD Radeon integrated GPU**, a saved GPU override, a per-user Windows installer,
and an optional launch-on-boot setting. It is a fork of the original vibranceGUI,
not an NVIDIA or AMD product.

[Get the installer](#get-the-windows-installer) ·
[Game setup](#set-up-digital-vibrance-for-cs2-valorant-or-another-game) ·
[GPU compatibility](#windows-and-gpu-compatibility) ·
[Troubleshooting and FAQ](#digital-vibrance-troubleshooting-and-faq)

## What's different in this fork?

- **NVIDIA + AMD iGPU support:** having both drivers installed is no longer treated
  as an error. Auto selection prefers NVIDIA.
- **Manual GPU selection:** choose Auto, NVIDIA, or AMD Radeon and restart to apply.
- **Independent vendor profiles:** NVIDIA vibrance and AMD saturation values are
  stored separately because they use different numeric scales.
- **Windows installer:** install for your account without administrator rights,
  with a Start Menu entry, optional desktop shortcut, and uninstaller.
- **Launch on boot:** optionally start minimized in the notification area at
  Windows sign-in.
- **Recoverable initialization errors:** GPU selection and startup controls remain
  available even when the chosen graphics driver cannot initialize.

## Windows and GPU compatibility

| Configuration | Support and limitations |
| --- | --- |
| Windows 11 / Windows 10 | Installer requires .NET Framework 4.8 or later and working vendor drivers. |
| NVIDIA GeForce | Requires a display connected to the NVIDIA GPU and driver support for Digital Vibrance. |
| NVIDIA GPU + AMD Ryzen CPU with Radeon integrated graphics | Auto prefers NVIDIA; an AMD override is available. Keep both drivers installed if both devices are in use. |
| AMD Radeon | Uses the AMD driver's saturation control where available. |
| Laptop / hybrid graphics | Depends on which GPU drives the display. A screen routed through an iGPU may not expose NVIDIA Digital Vibrance. |
| Intel graphics | Not supported by this application. |
| Multiple monitors | Only displays connected to the selected vendor's GPU can be controlled. |

**2026 validation:** on a Windows 11 machine with an RTX 5090 and Radeon iGPU,
automatic selection chose NVIDIA and the NVIDIA backend initialized with three
controllable displays. The AMD selection override was also detected successfully.
These were read-only driver checks, not a guarantee for every GPU, driver, HDR
configuration, or game. See the [verification notes](installer/VERIFICATION.md)
for the checks performed and remaining manual tests.

## Get the Windows installer

**Requirements:** Windows 10/11, .NET Framework 4.8 or later, and a supported
graphics driver. No developer tools or administrator rights are needed.

1. Download **[vibranceGUIx-Setup.exe](https://github.com/M-ax/vibranceGUIx/releases/latest/download/vibranceGUIx-Setup.exe)**.
2. Exit any running vibranceGUI or vibranceGUIx from its tray menu, then run Setup.
3. Choose whether to create a desktop shortcut and **Launch on boot (at sign-in)**.
4. Leave **Launch vibranceGUIx** selected on the final page to open the app.
5. Keep **Graphics driver → Auto (prefer NVIDIA)** unless you need the AMD backend.

Fresh installs use `%LocalAppData%\Programs\vibranceGUIx` for the current user;
upgrades retain the existing installation directory.
**The installer is unsigned**, so Windows may display an unknown-publisher or
SmartScreen warning. Download from this repository's releases and verify the
file before running it; do not disable Windows security software.

Prefer no installer? Download the
[portable ZIP](https://github.com/M-ax/vibranceGUIx/releases/latest/download/vibranceGUIx-Portable.zip),
extract it completely, and run `vibrance.GUI.exe`. Keep the `.config` file and DLL
alongside it. A quick-start guide and dependency notices are included.

### Verify your download

Download [SHA256SUMS.txt](https://github.com/M-ax/vibranceGUIx/releases/latest/download/SHA256SUMS.txt)
from the same release and compare its entry with:

```powershell
Get-FileHash .\vibranceGUIx-Setup.exe -Algorithm SHA256
```

Checksums detect a changed or incomplete download; they are not a code-signing
certificate. Older versions and their matching checksums remain on the
[release history](https://github.com/M-ax/vibranceGUIx/releases).

For source builds, see [Building and testing](#building-and-testing).
Downloads from the [original vibranceGUI website](https://vibrancegui.com/) are
separate builds and should not be assumed to include this fork's changes.

## Set up Digital Vibrance for CS2, Valorant, or another game

Profiles target a game's executable, not a hard-coded list of game titles.
For Counter-Strike 2 (CS2), select the current game process rather than an old
Counter-Strike: Global Offensive (CS:GO) entry. For Valorant, select the running
game rather than its launcher.

1. Set **Desktop vibrance** to your preferred desktop baseline.
2. Enable **Never change resolutions** if you want color adjustments only.
3. Start the game, click **Add**, and select its process. Alternatively, use
   **Add manually** to select the game executable.
4. Set **Ingame Vibrance Level** and click **Save**. Double-click an existing
   profile to edit it later.
5. Check game focus, Alt+Tab, and game exit separately. Confirm that your desired
   colors apply in-game and the desktop level returns afterward.

There is no universal "best digital vibrance setting": monitor calibration,
lighting, and personal preference matter. Start with a modest adjustment rather
than copying another player's value. This fork has not been end-to-end validated
in CS2 or Valorant, and it makes no blanket anti-cheat or tournament-approval
guarantee. Check the current rules for your game and competition.

## Choose NVIDIA or AMD and enable launch on boot

Use **Settings → Graphics driver → Auto (prefer NVIDIA), NVIDIA, or AMD Radeon**.
The choice is saved immediately; exit from the tray menu and reopen vibranceGUIx
to apply it. The window title shows the active backend, while the dropdown shows
your saved preference.

An unavailable explicit choice does not silently switch to another vendor.
Select Auto or another GPU to recover. This setting chooses the **display-color
backend**, not the GPU Windows uses to render your game.

**Launch on boot (at Windows sign-in)** is available in both Setup and the app.
It is opt-in and starts the app minimized in the notification area for your
account, not as a pre-login service. If Windows Task Manager has disabled the
startup entry, re-enable it there too.

## Digital Vibrance troubleshooting and FAQ

### Where is NVIDIA Digital Vibrance in Windows 11?

In NVIDIA Control Panel, open **Display → Adjust desktop color settings**, select
your monitor, and look for **Digital Vibrance**. Test the driver's own control
first: vibranceGUIx automates an existing driver feature rather than adding support
to an unsupported display.

NVIDIA's [desktop color settings documentation](https://www.nvidia.com/content/Control-Panel-Help/vLatest/en-us/mergedProjects/nvdsp/CS_Adjust_Color_Settings_Advanced.htm)
explains that **Override to reference mode** disables color enhancements. If it
is enabled, those controls are greyed out; consider whether you want to leave
reference mode before troubleshooting vibranceGUIx.

### Why does vibranceGUIx detect both NVIDIA and AMD drivers?

A Ryzen CPU with a Radeon iGPU and a dedicated GeForce card can legitimately have
both drivers installed. Older builds rejected this combination. This fork
instead prefers NVIDIA and lets you override the choice. You do not need to
uninstall an active iGPU's driver merely because both vendors are present.

### Why is Digital Vibrance not working on my laptop or second monitor?

Check which GPU physically drives that display. Rendering a game on NVIDIA does
not necessarily mean NVIDIA controls the screen's color output. A laptop's
internal screen or a monitor connected to the motherboard may be driven by the
integrated GPU. Choose the matching supported backend, or use a display connection
driven by the NVIDIA GPU where your hardware supports it.

If the app reports **GPU unavailable**, hover over the red status message for
details, check the vendor driver, then try Auto or the appropriate GPU override
and restart.

### Is this the same as NVIDIA RTX Dynamic Vibrance?

No. This application automates the driver's **Digital Vibrance** control on NVIDIA
and saturation on AMD. **RTX Dynamic Vibrance** is a separate AI-powered game
filter in the NVIDIA App; vibranceGUIx does not enable or configure that filter.
See [NVIDIA's explanation of its RTX filters](https://www.nvidia.com/en-us/geforce/news/february-22-2024-geforce-game-ready-driver/).

### Where are my settings, and can I switch GPU vendors?

For compatibility, vibranceGUIx retains the executable name `vibrance.GUI.exe`
and settings directory `%AppData%\vibranceGUI`. The vendor preference is in
`vibranceGUI.ini`; NVIDIA and AMD keep independent `vibranceGUI.<vendor>.ini` and
`applicationData.<vendor>.xml` profiles.

Existing untagged settings are imported only after you confirm which selected
vendor they belong to. Choose **No** and switch vendors first if necessary.
Original profile files are retained; uninstall also retains your settings.

## Building and testing

From Windows PowerShell at the repository root:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\installer\verify-payload.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\installer\verify-release.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tests\Run-Tests.ps1
```

The build requires Windows PowerShell 5.1, Visual Studio MSBuild with .NET desktop
build tools, and Internet access for the first build. The .NET SDK alone is not
sufficient. It uses
pinned local downloads rather than installing an old framework targeting pack.
It builds **Release/x86**, required by the bundled native NVIDIA bridge.
See the [build and packaging guide](installer/README.md) for prerequisites,
output files, startup/upgrade behavior and the install/uninstall acceptance
checklist.

On an NVIDIA + AMD hybrid PC, append `-Hardware` to the test command to check actual
driver selection and read-only NVIDIA Digital Vibrance initialization. This
hardware-specific check expects both vendors and does not change monitor colors.
The ordinary regression suite uses workspace fixtures and does not write startup
registry entries. Installer execution and real sign-in tests are separate manual
checks.

## Report an issue or contribute

Report problems with this fork in
[this repository's issue tracker](https://github.com/M-ax/vibranceGUIx/issues).
Include your Windows version, GPU model(s), driver versions, monitor connections,
selected backend, and steps to reproduce. State whether the driver's own color
control works and whether the problem occurs on game focus, Alt+Tab, or exit.

Contributions and reproducible bug reports are welcome. Do not report this fork's
installer or GPU-selection changes to the original authors as upstream bugs.

## Original project and credits

This C# / Windows Forms project builds on
[juv/vibranceGUI](https://github.com/juv/vibranceGUI).
The original project and downloads are at [vibrancegui.com](https://vibrancegui.com/).
Original NVIDIA work: [juvlarN](https://twitter.com/juvlarN).
Original AMD work: [juRiiir3](https://twitter.com/juRiiir3).
