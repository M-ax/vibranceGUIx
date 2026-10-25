# vibranceGUIx 2.3.1.3

**Independent fork, not an official release of the original vibranceGUI.**
Maintained by Max, based on [juv/vibranceGUI](https://github.com/juv/vibranceGUI).
For the original project and downloads, visit [vibrancegui.com](https://vibrancegui.com/).

The first downloadable release of this fork: NVIDIA Digital Vibrance and AMD
saturation profiles for Windows, with hybrid-GPU selection and a Windows installer.

## Downloads

- **vibranceGUIx-Setup.exe** — recommended. Per-user install, Start Menu shortcut,
  optional desktop shortcut and launch at Windows sign-in. No admin rights needed.
- **vibranceGUIx-Portable.zip** — extract everything, then run `vibrance.GUI.exe`.
- **SHA256SUMS.txt** — checksums for both downloads.

Requires Windows 10/11, .NET Framework 4.8+ and supported vendor graphics drivers.
The x86 application runs on both 32-bit and 64-bit Windows installations.
The installer and app are **unsigned**; Windows may show an unknown-publisher or
SmartScreen warning. Verify the release source and checksum. Do not disable
Windows security software.

## Highlights

- Automatically prefers NVIDIA when a Radeon CPU iGPU is also installed.
- Saved Auto / NVIDIA / AMD override, with independent vendor color profiles.
- Startup and GPU-selection controls stay usable if GPU initialization fails.
- Visible, opt-in launch-on-boot setting that starts minimized at sign-in.
- Clean product header and project/help link in place of promotional UI.
- Tray actions for Open, Project and help, and Exit; right-click opens the menu
  without unexpectedly restoring the window.
- Installer upgrade safeguards, owned-shortcut migration and startup-path handling.
- Quick-start instructions and dependency notices included in both distributions.

## Upgrade notes

Exit the running app before installing or replacing portable files. Existing
settings remain in `%AppData%\vibranceGUI`; the executable and startup registration
keep their legacy names for compatibility. Earlier installations retain their
install directory. GPU selection changes apply after exiting and reopening.

## Validation and limitations

Release/x86 build, regression/UI tests, payload checks and archive/checksum
verification passed. On the validation machine, Auto selected an RTX 5090
alongside a Radeon iGPU, NVIDIA Digital Vibrance initialized for three displays,
and the AMD selection override was detected. The driver probe was read-only.

Actual in-game color changes, AMD saturation changes, installer upgrade/uninstall
and sign-in startup have not been end-to-end verified. Laptop display routing,
HDR configurations and driver versions can affect support. No blanket CS2,
Valorant, anti-cheat or tournament compatibility guarantee is made.

Setup guide: https://github.com/M-ax/vibranceGUIx#readme

Report issues: https://github.com/M-ax/vibranceGUIx/issues

Based on the original [juv/vibranceGUI](https://github.com/juv/vibranceGUI).
