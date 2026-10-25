#requires -Version 5.1
param([string]$ReleaseDirectory = "$PSScriptRoot\..\artifacts\release")
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = Split-Path $PSScriptRoot -Parent
$payload = Join-Path $root 'artifacts\payload'
$expectedAssets = @('SHA256SUMS.txt', 'vibranceGUIx-Portable.zip', 'vibranceGUIx-Setup.exe')
$actualAssets = @(Get-ChildItem -LiteralPath $ReleaseDirectory | Select-Object -ExpandProperty Name)
if (Compare-Object $expectedAssets $actualAssets) { throw 'Release assets do not match the expected installer, portable ZIP and checksums.' }
$expectedNames = @('vibranceGUIx-Setup.exe', 'vibranceGUIx-Portable.zip')
$lines = @(Get-Content -LiteralPath (Join-Path $ReleaseDirectory 'SHA256SUMS.txt'))
if ($lines.Count -ne $expectedNames.Count) { throw 'Expected exactly two checksum entries.' }
foreach ($name in $expectedNames) {
    $hash = (Get-FileHash -LiteralPath (Join-Path $ReleaseDirectory $name) -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($lines -cnotcontains "$hash  $name") { throw "Missing or incorrect SHA256 checksum: $name" }
}

$version = [Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $payload 'vibrance.GUI.exe')).FileVersion
$builtInstaller = Join-Path $root "artifacts\vibranceGUIx-$version-Setup.exe"
if ((Get-FileHash $builtInstaller).Hash -ne (Get-FileHash (Join-Path $ReleaseDirectory 'vibranceGUIx-Setup.exe')).Hash) {
    throw 'Published installer candidate differs from the versioned build.'
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [IO.Compression.ZipFile]::OpenRead((Join-Path $ReleaseDirectory 'vibranceGUIx-Portable.zip'))
try {
    $expectedFiles = @('vibrance.GUI.exe', 'vibrance.GUI.exe.config', 'Microsoft.Practices.ServiceLocation.dll', 'README.txt', 'THIRD-PARTY-NOTICES.txt')
    $actualFiles = @($archive.Entries | ForEach-Object { $_.FullName })
    if (Compare-Object $expectedFiles $actualFiles) { throw 'Portable ZIP contains missing or unexpected files.' }
    if ($archive.Entries.Count -ne $expectedFiles.Count) { throw 'Portable ZIP contains duplicate entries.' }
    foreach ($entry in $archive.Entries) {
        $stream = $entry.Open()
        $sha = [Security.Cryptography.SHA256]::Create()
        try { $hash = [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-', '') }
        finally { $stream.Dispose(); $sha.Dispose() }
        if ($hash -ne (Get-FileHash (Join-Path $payload $entry.FullName)).Hash) {
            throw "Portable ZIP file differs from the tested payload: $($entry.FullName)"
        }
    }
} finally { $archive.Dispose() }
Write-Host "PASS: release checksums, installer identity, exact portable ZIP contents and payload hashes ($version)."
