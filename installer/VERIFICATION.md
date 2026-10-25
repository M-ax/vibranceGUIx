# vibranceGUIx packaging verification

## Validation environment

The branded vibranceGUIx 2.3.1.2 build was validated on Windows 11 build 26200 with Windows PowerShell 5.1,
Visual Studio 18 Community MSBuild, .NET Framework 4.8 runtime, and downloaded
Inno Setup 6.7.3. No tools were installed globally.

Commands completed successfully:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\installer\verify-payload.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tests\Run-Tests.ps1 -Hardware
git diff --check
```

- Automatic `vswhere` MSBuild discovery worked.
- Release/x86 build and Inno script compilation succeeded.
- Payload verification passed: x86 CLR flags, expected embedded resource name,
  embedded NVIDIA native DLL matching the source bytes, config and managed DLL.
- Existing application warnings remain: CS0659 (`ResolutionModeWrapper` missing
  `GetHashCode`) and CS0168 (unused `ex` in `WinEventHook`). No build errors.
- Regression and hardware checks passed 37 assertions: selection matrix, saved
  preference round-trips, invalid preference recovery, profile isolation/import,
  numeric range validation, recovery-window control availability, product/window
  branding, and preservation of the legacy startup registration.
- The actual Windows host selected NVIDIA automatically, selected AMD when
  overridden, and initialized the NVIDIA backend on its GeForce RTX 5090 with
  three controllable displays. This was a read-only driver probe; no saturation or
  vibrance settings were changed by the test.
- The recovery UI smoke test verified the startup checkbox and GPU dropdown are
  visible and enabled. The installer is unsigned.

## Final build artifacts and remaining checks

Produced artifact paths, relative to the repository root:

```text
artifacts/vibranceGUIx-2.3.1.2-Setup.exe
artifacts/payload/
artifacts/build.log
artifacts/build.binlog
artifacts/payload-sha256.json
```

Hashes and sizes identify a particular build, not all future builds. Source,
compiler and timestamp changes can change them; inspect the freshly generated
payload hash manifest and compute the installer hash for the build being tested.
These paths describe local build outputs, not a published release download.

The installer wizard was opened with permission, but completion is unknown.
Installation, uninstallation and sign-in startup are therefore **not verified**.
Actual in-game vibrance changes, AMD saturation changes, upgrade UI and
prerequisite failure without .NET 4.8 also remain unverified.
These require the [acceptance checks](README.md#manual-acceptance-checklist-requires-permission-or-a-disposable-vm) with explicit
permission or in a disposable Windows VM. Resource preservation alone does not
prove GPU driver compatibility.
