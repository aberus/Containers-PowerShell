---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Get-ContainerDetail.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Get-ContainerDetail
---

# Get-ContainerDetail

## SYNOPSIS

Returns the low-level configuration and state of a container.

## SYNTAX

### Default (Default)

```
Get-ContainerDetail [-ContainerIdOrName] <string[]> [-Session <Session>] [-SessionName <string>]
 [<CommonParameters>]
```

### ContainerObject

```
Get-ContainerDetail [-Container] <Container[]> [-Session <Session>] [-SessionName <string>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Returns the container’s full configuration and state as reported by the service, converted from JSON into an object.

## EXAMPLES

### Example 1

Inspects a container by id prefix

```powershell
PS C:\> Get-WslcContainerDetail 514e
```

## PARAMETERS

### -Container

The container objects to operate on, as output by Get-Container.

```yaml
Type: Microsoft.WSL.Containers.Container[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ContainerObject
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ContainerIdOrName

The names or ids of the containers to operate on. An id prefix is accepted as long as it matches only one container.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases:
- Name
- Id
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

### -Session

The WSLC session to operate on. When neither -Session nor -SessionName is given, the module’s default session is created or reused.

```yaml
Type: Microsoft.WSL.Containers.Session
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

### -SessionName

The name of the WSLC session to operate on, as registered by New-ContainerSession.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

The names or ids to operate on.

### Microsoft.WSL.Containers.Container[]

The containers to operate on.

## OUTPUTS

## NOTES

## RELATED LINKS

