#requires -Version 5.1
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$payload = Join-Path $root 'artifacts\payload'
# Reflection-only loading inspects resources without executing app startup.
$assembly = [Reflection.Assembly]::ReflectionOnlyLoadFrom((Join-Path $payload 'vibrance.GUI.exe'))
$kind = [Reflection.PortableExecutableKinds]0
$machine = [Reflection.ImageFileMachine]::I386
$assembly.ManifestModule.GetPEKind([ref]$kind, [ref]$machine)
if ($machine -ne [Reflection.ImageFileMachine]::I386 -or
    !($kind -band [Reflection.PortableExecutableKinds]::Required32Bit)) {
    throw "Expected x86 assembly; found $machine / $kind"
}
$resource = $assembly.GetManifestResourceStream('vibrance.GUI.NVIDIA.vibranceDLL.dll')
if (!$resource) { throw 'Missing embedded NVIDIA native DLL' }
$sha = [Security.Cryptography.SHA256]::Create()
try { $embeddedHash = [BitConverter]::ToString($sha.ComputeHash($resource)).Replace('-', '') }
finally { $resource.Dispose(); $sha.Dispose() }
if ($embeddedHash -ne (Get-FileHash (Join-Path $root 'vibrance.GUI\NVIDIA\vibranceDLL.dll')).Hash) {
    throw 'Embedded NVIDIA DLL does not match source DLL'
}
foreach ($file in @('vibrance.GUI.exe.config', 'Microsoft.Practices.ServiceLocation.dll')) {
    if (!(Test-Path (Join-Path $payload $file))) { throw "Missing payload file: $file" }
}
Write-Host "PASS: x86 CLR assembly, exact NVIDIA native resource, config and managed dependency present."
