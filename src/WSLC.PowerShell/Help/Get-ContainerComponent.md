---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Get-ContainerComponent.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Get-ContainerComponent
---

# Get-ContainerComponent

## SYNOPSIS

Returns the WSLC components that are missing.

## SYNTAX

### __AllParameterSets

```
Get-ContainerComponent [-Session <Session>] [-SessionName <string>] [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Returns the components WSLC reports as missing, which is what Install-ContainerFeature installs when it is run without -Component.

## EXAMPLES

### Example 1

Lists the components that still need installing

```powershell
PS C:\> Get-WslcContainerComponent
```

## PARAMETERS

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

## OUTPUTS

### Microsoft.WSL.Containers.Component

## NOTES

## RELATED LINKS

