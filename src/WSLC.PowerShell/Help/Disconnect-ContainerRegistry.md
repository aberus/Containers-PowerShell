---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Disconnect-ContainerRegistry.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Disconnect-ContainerRegistry
---

# Disconnect-ContainerRegistry

## SYNOPSIS

Forgets the stored token for a container registry.

## SYNTAX

### Default (Default)

```
Disconnect-ContainerRegistry [[-Server] <string>] [-Session <Session>] [-SessionName <string>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### All

```
Disconnect-ContainerRegistry -All [-Session <Session>] [-SessionName <string>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases: "Logout-ContainerRegistry".

## DESCRIPTION

Forgets the token stored by Connect-ContainerRegistry, so later pulls and pushes to that registry are anonymous again.

## EXAMPLES

### Example 1

Forgets the token stored for Docker Hub

```powershell
PS C:\> Disconnect-WslcContainerRegistry
```

### Example 2

Forgets every stored registry token in the session

```powershell
PS C:\> Disconnect-WslcContainerRegistry -All
```

## PARAMETERS

### -All

Forgets the stored tokens for every registry in the session.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: All
  Position: Named
  IsRequired: true
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

### -Server

The registry to forget the stored token for. Defaults to Docker Hub.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- ServerAddress
- Registry
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

### System.String

The name or id to operate on.

## OUTPUTS

## NOTES

## RELATED LINKS

