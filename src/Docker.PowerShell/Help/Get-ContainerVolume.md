---
document type: cmdlet
external help file: Docker.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/Docker.PowerShell/Help/Get-ContainerVolume.md
Locale: en-US
Module Name: Docker
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Get-ContainerVolume
---

# Get-ContainerVolume

## SYNOPSIS

Gets the volumes on the daemon.

## SYNTAX

### Default (Default)

```
Get-ContainerVolume [[-Name] <string[]>] [-HostAddress <string>] [-Context <string>]
 [-CertificateLocation <string>] [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Gets the volumes on the daemon.  If no name is specified, every volume is returned.  Any warnings the daemon reports while listing (for example a volume driver it could not reach) are written to the warning stream.

## EXAMPLES

### Example 1

Gets every volume on the daemon.

```powershell
PS C:\> Get-ContainerVolume
```

### Example 2

Gets the volume named "db-data".  Names are matched exactly, so this does not also return a volume named "db-data-backup".

```powershell
PS C:\> Get-ContainerVolume -Name db-data
```

### Example 3

Removes every volume that uses the "local" driver.

```powershell
PS C:\> Get-ContainerVolume | Where-Object Driver -eq local | Remove-ContainerVolume
```

## PARAMETERS

### -CertificateLocation

The location of the X509 certificate file named "key.pfx" that will be used for authentication with the server. (Note that certificate authorization work is still in progress and this is likely to change).

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Context

The name of the docker context to connect through. The context supplies the endpoint and any TLS material.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -HostAddress

The address of the docker daemon to connect to.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Name

The names of the volumes to get.  If not specified, every volume is returned.  A name that matches no volume returns nothing rather than an error.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: 0
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### Docker.DotNet.Models.VolumeResponse

## NOTES

## RELATED LINKS

