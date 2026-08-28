$projectFile = "src\Docker.PowerShell\Docker.PowerShell.csproj"
$projectRawContents = Get-Content $projectFile -Raw
$versionNodes =  $projectRawContents | Select-Xml -XPath "/Project/PropertyGroup/Version"
if ($versionNodes.Count -ne 1) {
  throw "Invalid number of version nodes found in csproj."
}

$version = $versionNodes.Node.InnerXml

# Tags do not get build versions.
if ($env:APPVEYOR_REPO_TAG -ne "true") {
  $version += ".$($env:APPVEYOR_BUILD_VERSION.split("-")[0])"

  # Update the .csproj version to include the build number.
  $newProjectContents = $projectRawContents -replace "<Version>.+</Version>", "<Version>$version</Version>"
  $newProjectContents | Out-File -Encoding UTF8 $projectFile
}

# Replace module manifest version.
$manifest = "src\Docker.PowerShell\Docker.psd1"
(Get-Content $manifest -Raw) -replace "ModuleVersion.+","ModuleVersion = '$version'" | Out-File $manifest

dotnet restore src/Docker.PowerShell/Docker.PowerShell.csproj
dotnet build src/Docker.PowerShell/Docker.PowerShell.csproj
dotnet publish -f net462 -o $pwd\bin\Docker\clr -c Release $pwd\src\Docker.PowerShell
dotnet publish -f netstandard2.0 -o $pwd\bin\Docker\coreclr -c Release $pwd\src\Docker.PowerShell
#nuget install Newtonsoft.Json -Version 9.0.1 -OutputDirectory $pwd\bin
Copy-Item $pwd\bin\Docker\coreclr\Docker.*ps* $pwd\bin\Docker\ -Force
#cp $pwd\bin\Newtonsoft.Json.9.0.1\lib\portable-net45+wp80+win8+wpa81\Newtonsoft.Json.dll $pwd\bin\Docker\coreclr\Newtonsoft.Json.dll
New-ExternalHelp -Path src\Docker.PowerShell\Help -OutputPath $pwd\bin\Docker\en-US

if (!(Test-Path $pwd\testRepo)) {
    New-Item $pwd\testRepo -ItemType Directory
    Register-PSRepository -Name test -SourceLocation $pwd\testRepo
}
Publish-PSResource -Path $pwd\bin\Docker -Repository test

Install-PSResource -Name Docker -Repository test -Reinstall -TrustRepository
#Import-Module Docker
# if (!(Get-Command -Module Docker)){
#     throw "Module failed to load: no commands found."
# }


# New-MarkdownHelp -Module Docker -OutputFolder src\Docker.PowerShell\Help -ErrorAction SilentlyContinue
# Update-MarkdownHelp -Path src\Docker.PowerShell\Help
