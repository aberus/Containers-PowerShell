---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Wait-Container.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Wait-Container
---

# Wait-Container

## SYNOPSIS

Waits for a container's init process to exit.

## SYNTAX

### Default (Default)

```
Wait-Container [-ContainerIdOrName] <string[]> [-PassThru] [-Session <Session>]
 [-SessionName <string>] [<CommonParameters>]
```

### ContainerObject

```
Wait-Container [-Container] <Container[]> [-PassThru] [-Session <Session>] [-SessionName <string>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Waits for the container’s init process to exit and writes its exit code to the verbose stream. A non-zero exit code is reported as an error.

## EXAMPLES

### Example 1

Waits for a container to exit

```powershell
PS C:\> Wait-WslcContainer build
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

### -PassThru

If specified, the container object is written to the pipeline once it has exited.

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

### Microsoft.WSL.Containers.Container

## NOTES

## RELATED LINKS

