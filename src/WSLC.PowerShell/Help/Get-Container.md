---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Get-Container.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Get-Container
---

# Get-Container

## SYNOPSIS

Returns a list of containers.

## SYNTAX

### Default (Default)

```
Get-Container [[-ContainerIdOrName] <string[]>] [-Session <Session>] [-SessionName <string>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Returns the containers this module created or opened, including those created by an earlier PowerShell process, which are re-opened from the module’s own record. The WSLC SDK cannot enumerate a session’s containers, so a container created outside this module is only returned when it is named explicitly.

## EXAMPLES

### Example 1

Lists every container this module knows about

```powershell
PS C:\> Get-WslcContainer
```

### Example 2

Gets one container by name

```powershell
PS C:\> Get-WslcContainer web
```

## PARAMETERS

### -ContainerIdOrName

The names or ids of the containers to get. An id prefix is accepted as long as it matches only one container. When omitted, every known container is returned.

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
  IsRequired: false
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

## OUTPUTS

### Microsoft.WSL.Containers.Container

## NOTES

## RELATED LINKS

