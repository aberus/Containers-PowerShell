---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Get-ContainerSession.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Get-ContainerSession
---

# Get-ContainerSession

## SYNOPSIS

Returns the WSLC sessions tracked in this PowerShell process.

## SYNTAX

### Default (Default)

```
Get-ContainerSession [[-Name] <string[]>] [-Session <Session>] [-SessionName <string>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Returns the WSLC sessions registered in this PowerShell process. Sessions are process-local: one created in another process is not listed here.

## EXAMPLES

### Example 1

Lists the sessions registered in this process

```powershell
PS C:\> Get-WslcContainerSession
```

## PARAMETERS

### -Name

The names of the sessions to get. When omitted, every session registered in this process is returned.

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

### Microsoft.WSL.Containers.Session

## NOTES

## RELATED LINKS

