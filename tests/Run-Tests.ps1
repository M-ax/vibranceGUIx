param(
    [string]$AppPath = "$PSScriptRoot\..\vibrance.GUI\bin\x86\Release\vibrance.GUI.exe",
    [switch]$Hardware,
    [string]$ScreenshotPath
)
$ErrorActionPreference = 'Stop'

# The application and native bridge are x86 .NET Framework, not .NET/Core.
if ([IntPtr]::Size -ne 4 -or $PSVersionTable.PSEdition -eq 'Core') {
    $arguments = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $PSCommandPath, '-AppPath', $AppPath)
    if ($Hardware) { $arguments += '-Hardware' }
    if ($ScreenshotPath) { $arguments += @('-ScreenshotPath', $ScreenshotPath) }
    & "$env:WINDIR\SysWOW64\WindowsPowerShell\v1.0\powershell.exe" @arguments
    exit $LASTEXITCODE
}

$app = (Resolve-Path $AppPath).Path
$assembly = [Reflection.Assembly]::LoadFrom($app)
Add-Type -AssemblyName System.Windows.Forms, System.Drawing
$fixtures = Join-Path $PSScriptRoot "..\artifacts\tests\$([Guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $fixtures -Force | Out-Null
$testAssembly = Join-Path $fixtures 'RegressionTests.dll'
& "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\csc.exe" /nologo /target:library /platform:x86 "/out:$testAssembly" "/reference:$app" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll "$PSScriptRoot\RegressionTests.cs"
if ($LASTEXITCODE -ne 0) { throw 'Regression test compilation failed.' }
[void][Reflection.Assembly]::LoadFrom($testAssembly)
[RegressionTests]::Run($assembly, $fixtures, $Hardware.IsPresent,
    (Resolve-Path "$PSScriptRoot\..\vibrance.GUI\NVIDIA\vibranceDLL.dll").Path, $ScreenshotPath)
