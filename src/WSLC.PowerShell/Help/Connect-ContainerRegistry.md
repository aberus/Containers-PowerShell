---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Connect-ContainerRegistry.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Connect-ContainerRegistry
---

# Connect-ContainerRegistry

## SYNOPSIS

Authenticates with a container registry.

## SYNTAX

### __AllParameterSets

```
Connect-ContainerRegistry [[-Server] <string>] [-Credential] <pscredential> [-PassThru]
 [-Session <Session>] [-SessionName <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases: "Login-ContainerRegistry".

## DESCRIPTION

Authenticates with a container registry and remembers the identity token it returns, so that later pulls and pushes to that registry pick it up without -RegistryAuth. The token is held for the session in this PowerShell process only and is never written to disk.

## EXAMPLES

### Example 1

Authenticates with Docker Hub, prompting for the credential

```powershell
PS C:\> Connect-WslcContainerRegistry -Credential (Get-Credential)
```

### Example 2

Authenticates with a private registry and shows the token that was stored

```powershell
PS C:\> Connect-WslcContainerRegistry -Server ghcr.io -Credential $cred -PassThru
```

## PARAMETERS

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

### -Credential

The credential to authenticate with. PowerShell prompts for it when it is not supplied as a PSCredential.

```yaml
Type: System.Management.Automation.PSCredential
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PassThru

If specified, the authentication result is written to the pipeline. It carries a credential, so it is withheld by default.

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

### -Server

The registry to authenticate with; a bare host name assumes https. Defaults to Docker Hub.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- ServerAddress
- Registry
ParameterSets:
- Name: (All)
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

### Microsoft.WSL.Containers.AuthenticateResult

## NOTES

## RELATED LINKS

