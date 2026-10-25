#requires -Version 5.1
[CmdletBinding()]
param(
    [string]$MSBuildPath,
    [switch]$SkipInstaller
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = $PSScriptRoot
$tools = Join-Path $root '.tools'
$artifacts = Join-Path $root 'artifacts'
New-Item -ItemType Directory -Force $tools, $artifacts | Out-Null
# Never leave a previous release candidate available after a partial/failed build.
$release = Join-Path $artifacts 'release'
if (Test-Path $release) { Remove-Item $release -Recurse -Force }
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Restore-PinnedPackage($id, $version, $hash, $destination) {
    $archive = Join-Path $tools "$id.$version.zip"
    if (!(Test-Path $archive)) {
        Invoke-WebRequest "https://api.nuget.org/v3-flatcontainer/$id/$version/$id.$version.nupkg" -UseBasicParsing -OutFile $archive
    }
    if ((Get-FileHash $archive -Algorithm SHA256).Hash -ne $hash) {
        throw "SHA256 mismatch: $archive. Remove it and retry."
    }
    # Always extract the verified archive; don't trust stale extracted tools.
    if (Test-Path $destination) { Remove-Item $destination -Recurse -Force }
    Expand-Archive $archive $destination
}

Restore-PinnedPackage 'commonservicelocator' '1.3.0' 'B6826C427C0223FF7E8F341E3441DA6D1B8E0D172FDB405054BA6BFB76917078' (Join-Path $root 'packages\CommonServiceLocator.1.3')
# NuGet decodes escaped entry names; Expand-Archive deliberately does not.
$library = Join-Path $root 'packages\CommonServiceLocator.1.3\lib'
Get-ChildItem $library -Directory | Where-Object { $_.Name.Contains('%') } |
    Rename-Item -NewName { [Uri]::UnescapeDataString($_.Name) }
Restore-PinnedPackage 'microsoft.netframework.referenceassemblies.net40' '1.0.3' '54D6E20A1B61CAF79395D6D71D091265E81F5A5705AC4AE52AF45CA143C3C694' (Join-Path $root 'packages\Microsoft.NETFramework.ReferenceAssemblies.net40.1.0.3')

if (!$MSBuildPath) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $MSBuildPath = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
    }
}
if (!$MSBuildPath -or !(Test-Path $MSBuildPath)) {
    throw 'Visual Studio MSBuild is required. Supply -MSBuildPath; dotnet build cannot build this legacy WinForms project.'
}
$payload = Join-Path $artifacts 'payload'
if (Test-Path $payload) { Remove-Item $payload -Force -Recurse }
New-Item -ItemType Directory $payload | Out-Null
$project = Join-Path $root 'vibrance.GUI\vibrance.GUI.csproj'
& $MSBuildPath $project /t:Rebuild /p:Configuration=Release /p:Platform=x86 /nologo /v:minimal "/bl:$artifacts\build.binlog" "/flp:logfile=$artifacts\build.log;verbosity=normal"
if ($LASTEXITCODE -ne 0) { throw "MSBuild failed: $LASTEXITCODE" }
$output = Join-Path $root 'vibrance.GUI\bin\x86\Release'
foreach ($file in @('vibrance.GUI.exe', 'vibrance.GUI.exe.config', 'Microsoft.Practices.ServiceLocation.dll')) {
    Copy-Item (Join-Path $output $file) $payload
}
Copy-Item (Join-Path $root 'installer\PORTABLE-README.txt') (Join-Path $payload 'README.txt')
Copy-Item (Join-Path $root 'installer\THIRD-PARTY-NOTICES.txt') $payload
$exe = Join-Path $payload 'vibrance.GUI.exe'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exe).FileVersion
if ($version -notmatch '^\d+\.\d+\.\d+\.\d+$') { throw "Invalid file version: $version" }
if (!$SkipInstaller) {
    $inno = Join-Path $tools 'Tools.InnoSetup.6.7.3'
    Restore-PinnedPackage 'tools.innosetup' '6.7.3' 'F780898E402FF80612CC8D9FCB8C6E02932BD1CB4C900FFDAA31F9341CFB49F4' $inno
    $iscc = Join-Path $inno 'tools\ISCC.exe'
    & $iscc "/DAppVersion=$version" "/DPayloadDir=$payload" "/DOutputDir=$artifacts" (Join-Path $root 'installer\vibranceGUI.iss')
    if ($LASTEXITCODE -ne 0) { throw "ISCC failed: $LASTEXITCODE" }

    # Stable asset names keep /releases/latest/download links valid across versions.
    New-Item -ItemType Directory $release | Out-Null
    Copy-Item (Join-Path $artifacts "vibranceGUIx-$version-Setup.exe") (Join-Path $release 'vibranceGUIx-Setup.exe')
    Compress-Archive -Path (Join-Path $payload '*') -DestinationPath (Join-Path $release 'vibranceGUIx-Portable.zip')
    $checksums = foreach ($name in @('vibranceGUIx-Setup.exe', 'vibranceGUIx-Portable.zip')) {
        '{0}  {1}' -f (Get-FileHash (Join-Path $release $name) -Algorithm SHA256).Hash.ToLowerInvariant(), $name
    }
    $checksums | Set-Content (Join-Path $release 'SHA256SUMS.txt') -Encoding ASCII
}
Get-ChildItem $payload -File | Get-FileHash -Algorithm SHA256 |
    Select-Object @{n='File';e={Split-Path $_.Path -Leaf}}, Hash |
    ConvertTo-Json | Set-Content (Join-Path $artifacts 'payload-sha256.json') -Encoding UTF8
Write-Host "Release x86 payload: $payload"
if (!$SkipInstaller) { Write-Host "Installer: $artifacts\vibranceGUIx-$version-Setup.exe" }
if (!$SkipInstaller) { Write-Host "Release assets: $artifacts\release" }
