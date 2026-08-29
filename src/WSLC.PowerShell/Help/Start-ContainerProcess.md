---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Start-ContainerProcess.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Start-ContainerProcess
---

# Start-ContainerProcess

## SYNOPSIS

Runs a command inside a running container.

## SYNTAX

### Default (Default)

```
Start-ContainerProcess [-ContainerIdOrName] <string> [-Command] <string[]> [-Detached]
 [-WorkingDirectory <string>] [-Environment <string[]>] [-Session <Session>] [-SessionName <string>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ContainerObject

```
Start-ContainerProcess [-Container] <Container> [-Command] <string[]> [-Detached]
 [-WorkingDirectory <string>] [-Environment <string[]>] [-Session <Session>] [-SessionName <string>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases: "Exec-Container".

## DESCRIPTION

Runs a command inside a container that is already running, the way `wslc container exec` does. The output is streamed to the console unless -Detached is given.

## EXAMPLES

### Example 1

Runs a command in a running container

```powershell
PS C:\> Start-WslcContainerProcess web -- ls /etc
```

### Example 2

Starts a background process and returns the process object

```powershell
PS C:\> Start-WslcContainerProcess web -Detached -- sleep 60
```

## PARAMETERS

### -Command

The command to run inside the container.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: true
- Name: ContainerObject
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: true
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

### -Container

The container to run the command in, as output by Get-Container.

```yaml
Type: Microsoft.WSL.Containers.Container
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

The name or id of the container to run the command in.

```yaml
Type: System.String
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

### -Detached

Starts the process without streaming its output, writing the process object to the pipeline instead.

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

### -Environment

Environment variables for the process, in "NAME=VALUE" form.

```yaml
Type: System.String[]
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

### -WorkingDirectory

The working directory for the process.

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

### System.String

The name or id to operate on.

### Microsoft.WSL.Containers.Container

The container to operate on.

## OUTPUTS

### Microsoft.WSL.Containers.Process

## NOTES

## RELATED LINKS

