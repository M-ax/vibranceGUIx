#ifndef AppVersion
  #error Build using build.ps1 to supply the application version and verified payload.
#endif

[Setup]
AppId={{97382C81-0266-43CB-ACF2-D0D6F5AA532A}
AppName=vibranceGUIx
AppVersion={#AppVersion}
AppPublisher=vibranceGUIx contributors
AppPublisherURL=https://github.com/M-ax/vibranceGUIx
AppSupportURL=https://github.com/M-ax/vibranceGUIx/issues
DefaultDirName={localappdata}\Programs\vibranceGUIx
DefaultGroupName=vibranceGUIx
UsePreviousAppDir=yes
PrivilegesRequired=lowest
MinVersion=10.0
OutputDir={#OutputDir}
OutputBaseFilename=vibranceGUIx-{#AppVersion}-Setup
UninstallDisplayIcon={app}\vibrance.GUI.exe
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
CloseApplicationsFilter=*.exe,*.dll
AppMutex=vibranceGUI~Mutex
RestartApplications=no
UsePreviousTasks=no
DisableProgramGroupPage=yes
SetupIconFile=..\vibrance.GUI\setting.ico

[Tasks]
Name: desktopicon; Description: "Create a desktop shortcut"; Flags: unchecked
Name: startup; Description: "Launch on boot (at sign-in)"; Flags: unchecked; Check: not StartupEnabled
Name: startup; Description: "Launch on boot (at sign-in)"; Check: StartupEnabled

[Files]
Source: "{#PayloadDir}\vibrance.GUI.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#PayloadDir}\vibrance.GUI.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#PayloadDir}\Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{userprograms}\vibranceGUIx"; Filename: "{app}\vibrance.GUI.exe"; WorkingDir: "{app}"
Name: "{userdesktop}\vibranceGUIx"; Filename: "{app}\vibrance.GUI.exe"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\vibrance.GUI.exe"; Description: "Launch vibranceGUIx"; Flags: nowait postinstall skipifsilent unchecked

[Code]
const
  RunKey = 'SOFTWARE\Microsoft\Windows\CurrentVersion\Run';

var
  InitialStartupCommand: String;

function StartupCommand: String;
begin
  Result := '"' + ExpandConstant('{app}\vibrance.GUI.exe') + '" -minimized';
end;

function StartupEnabled: Boolean;
begin
  { The live registry, not last install's task choice, is the source of truth. }
  Result := InitialStartupCommand <> '';
end;

procedure RemoveOwnedStartup;
var
  Value: String;
begin
  { Never delete another installation's startup registration. }
  if RegQueryStringValue(HKCU, RunKey, 'vibranceGUI', Value) and
     (CompareText(Value, StartupCommand) = 0) then
    RegDeleteValue(HKCU, RunKey, 'vibranceGUI');
end;

function InitializeSetup: Boolean;
var
  Release: Cardinal;
begin
  RegQueryStringValue(HKCU, RunKey, 'vibranceGUI', InitialStartupCommand);
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and
    (Release >= 528040);
  if not Result then
    MsgBox('vibranceGUIx requires .NET Framework 4.8 or later. Install it from Microsoft, then run Setup again.', mbError, MB_OK);
end;

procedure RemoveLegacyShortcut(Path: String);
var
  Shell, Shortcut: Variant;
begin
  if not FileExists(Path) then
    Exit;
  try
    Shell := CreateOleObject('WScript.Shell');
    Shortcut := Shell.CreateShortcut(Path);
    { Preserve any original vibranceGUI or portable copy's shortcuts. }
    if CompareText(Shortcut.TargetPath, ExpandConstant('{app}\vibrance.GUI.exe')) = 0 then
      if not DeleteFile(Path) then
        Log('Could not remove legacy shortcut: ' + Path);
  except
    Log('Could not inspect legacy shortcut; leaving it unchanged: ' + Path);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  Value: String;
begin
  if CurStep = ssPostInstall then begin
    RemoveLegacyShortcut(ExpandConstant('{userprograms}\vibranceGUI.lnk'));
    RemoveLegacyShortcut(ExpandConstant('{userdesktop}\vibranceGUI.lnk'));
    if WizardIsTaskSelected('startup') then begin
      if not RegWriteStringValue(HKCU, RunKey, 'vibranceGUI', StartupCommand) then
        MsgBox('Could not enable launch at sign-in. Enable it from vibranceGUIx settings.', mbError, MB_OK);
    end else begin
      { Honor the user's opt-out, including an older portable copy, but don't
        overwrite a registration changed by another process during setup. }
      if RegQueryStringValue(HKCU, RunKey, 'vibranceGUI', Value) and
         (CompareText(Value, InitialStartupCommand) = 0) then
        if not RegDeleteValue(HKCU, RunKey, 'vibranceGUI') then
          MsgBox('Could not disable launch at sign-in. Disable it from vibranceGUIx settings.', mbError, MB_OK);
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
    RemoveOwnedStartup;
end;
