---
document type: cmdlet
external help file: Docker.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/Docker.PowerShell/Help/Remove-ContainerVolume.md
Locale: en-US
Module Name: Docker
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Remove-ContainerVolume
---

# Remove-ContainerVolume

## SYNOPSIS

Removes one or more volumes.

## SYNTAX

### Default (Default)

```
Remove-ContainerVolume [-Name] <string[]> [-Force] [-HostAddress <string>] [-Context <string>]
 [-CertificateLocation <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### VolumeObject

```
Remove-ContainerVolume [-Volume] <VolumeResponse[]> [-Force] [-HostAddress <string>]
 [-Context <string>] [-CertificateLocation <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Removes one or more volumes, specified either by name or as the volume objects returned by Get-ContainerVolume.  The daemon refuses to remove a volume that is still in use by a container.

## EXAMPLES

### Example 1

Removes the volume named "db-data".

```powershell
PS C:\> Remove-ContainerVolume -Name db-data
```

### Example 2

Removes every volume that uses the "local" driver.

```powershell
PS C:\> Get-ContainerVolume | Where-Object Driver -eq local | Remove-ContainerVolume
```

### Example 3

Removes the volume named "db-data", succeeding even if that volume no longer exists.

```powershell
PS C:\> Remove-ContainerVolume -Name db-data -Force
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

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases:
- cf
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

### -Force

Forces the removal of the volume, and answers the confirmation prompt about losing the data it holds. As with `docker volume rm --force`, a volume that is already gone is reported as success instead of an error.

```yaml
Type: System.Management.Automation.SwitchParameter
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

The names of the volumes to remove.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Volume

The volume objects to remove, as returned by Get-ContainerVolume.

```yaml
Type: Docker.DotNet.Models.VolumeResponse[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: VolumeObject
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf

Shows what would happen if the cmdlet runs. The cmdlet is not run.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases:
- wi
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

### Docker.DotNet.Models.VolumeResponse[]

## OUTPUTS

### None

## NOTES

The docker API also accepts a force flag on the remove request itself.  Docker.DotNet 4.3.3 drops that argument before building the request, so -Force is applied by this cmdlet instead.

## RELATED LINKS

