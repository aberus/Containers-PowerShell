<#
.SYNOPSIS
Dumps the public members of Microsoft.WSL.Containers SDK types, for diffing SDK versions.

.EXAMPLE
./inspect-wslc.ps1 -Version 2.9.9
#>
[CmdletBinding()]
param(
    # The Microsoft.WSL.Containers package version to inspect.
    [ValidateNotNullOrEmpty()]
    [string] $Version = '2.9.9',

    # The types to dump.
    [ValidateNotNullOrEmpty()]
    [string[]] $TypeName = @(
        'Microsoft.WSL.Containers.WslcService',
        'Microsoft.WSL.Containers.ServiceVersion',
        'Microsoft.WSL.Containers.InstallProgress'
    )
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$basePath = Join-Path $env:USERPROFILE ".nuget\packages\microsoft.wsl.containers\$Version\lib\net8.0-windows10.0.19041.0"
$assemblyPath = Join-Path $basePath 'wslcsdkcs.dll'
if (-not (Test-Path -LiteralPath $assemblyPath)) {
    throw "Microsoft.WSL.Containers $Version is not in the NuGet cache: $assemblyPath"
}

$assembly = [System.Runtime.Loader.AssemblyLoadContext]::Default.LoadFromAssemblyPath($assemblyPath)

$propertyFlags = [Reflection.BindingFlags] 'Public, Instance, Static'
$methodFlags = [Reflection.BindingFlags] 'Public, Instance, Static, DeclaredOnly'

foreach ($name in $TypeName) {
    $type = $assembly.GetType($name, $true)
    "TYPE $($type.FullName)"

    foreach ($property in $type.GetProperties($propertyFlags)) {
        "  PROP $($property.PropertyType.FullName) $($property.Name)"
    }

    foreach ($method in $type.GetMethods($methodFlags)) {
        $parameters = ($method.GetParameters() |
            ForEach-Object { "$($_.ParameterType.FullName) $($_.Name)" }) -join ', '
        "  METHOD $($method.ReturnType.FullName) $($method.Name)($parameters)"
    }
}
