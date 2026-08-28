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
# Compile the help markdown (Microsoft.PowerShell.PlatyPS, schema 2024-05-01) into the MAML
# the module loads at runtime. Only the command files are compiled; Help\Docker.md is the
# module landing page. Export-MamlCommandHelp writes into a folder named for the module, so
# the result is copied up into en-US.
Import-Module Microsoft.PowerShell.PlatyPS
$helpMarkdown = (Measure-PlatyPSMarkdown -Path $pwd\src\Docker.PowerShell\Help\*.md |
    Where-Object FileType -match 'CommandHelp').FilePath
$mamlStaging = Join-Path $pwd "bin\maml"
$null = Export-MamlCommandHelp -CommandHelp (Import-MarkdownCommandHelp -Path $helpMarkdown) -OutputFolder $mamlStaging -Force
$null = New-Item -ItemType Directory -Force -Path $pwd\bin\Docker\en-US
Copy-Item (Join-Path $mamlStaging "Docker\*.xml") $pwd\bin\Docker\en-US -Force
Remove-Item $mamlStaging -Recurse -Force

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


# To regenerate the help markdown from scratch (this discards the hand-written prose):
# New-MarkdownCommandHelp -ModuleInfo (Get-Module Docker) -OutputFolder src\Docker.PowerShell\Help -WithModulePage
