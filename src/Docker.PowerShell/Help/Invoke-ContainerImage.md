---
document type: cmdlet
external help file: Docker.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/Docker.PowerShell/Help/Invoke-ContainerImage.md
Locale: en-US
Module Name: Docker
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Invoke-ContainerImage
---

# Invoke-ContainerImage

## SYNOPSIS

Runs a container from an existing image. Aliased as "Run-Container".

## SYNTAX

### Default (Default)

```
Invoke-ContainerImage [-ImageIdOrName] <string[]> [[-Command] <string[]>] [-RemoveAutomatically]
 [-PassThru] [-Detach] [-Name <string>] [-Publish <string[]>] [-Isolation <IsolationType>]
 [-Configuration <ContainerConfig>] [-HostConfiguration <HostConfig>] [-Input] [-Terminal]
 [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ImageObject

```
Invoke-ContainerImage [-Image] <ImagesListResponse[]> [[-Command] <string[]>] [-RemoveAutomatically]
 [-PassThru] [-Detach] [-Name <string>] [-Publish <string[]>] [-Isolation <IsolationType>]
 [-Configuration <ContainerConfig>] [-HostConfiguration <HostConfig>] [-Input] [-Terminal]
 [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ConfigObject

```
Invoke-ContainerImage [-Configuration] <ContainerConfig> [-RemoveAutomatically] [-PassThru]
 [-Detach] [-HostConfiguration <HostConfig>] [-HostAddress <string>] [-Context <string>]
 [-CertificateLocation <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ContainerObject

```
Invoke-ContainerImage [-RemoveAutomatically] [-PassThru] [-Detach] [-HostAddress <string>]
 [-Context <string>] [-CertificateLocation <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

Run-Container, Run-ContainerImage

## DESCRIPTION

Runs a container from an existing image. Aliased as "Run-Container".

## EXAMPLES

### Example 1

Starts a container using the image with id 903ef.

```powershell
PS C:\> Invoke-ContainerImage -ImageIdOrName 903ef
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

### -Command

Specifies the command to run in the container.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: 1
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: true
- Name: ImageObject
  Position: 1
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: true
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Configuration

An instance of a Docker.DotNet.Models.Config object with desired configuration values filled out.

```yaml
Type: Docker.DotNet.Models.ContainerConfig
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ImageObject
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ConfigObject
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
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

The name of a docker context to connect through. The context supplies the endpoint and any TLS material, so it is an alternative to giving -HostAddress and -CertificateLocation yourself.

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

### -Detach

Run the container in detached mode, which skips attaching to input/output pipes.

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

### -HostConfiguration

A Docker.DotNet.Models.HostConfig object filled in with desired configuration values.

```yaml
Type: Docker.DotNet.Models.HostConfig
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ImageObject
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ConfigObject
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Image

Specifies the container image.

```yaml
Type: Docker.DotNet.Models.ImagesListResponse[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ImageObject
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ImageIdOrName

The Id or Name of the image the container is created from.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases:
- ImageName
- ImageId
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

### -Input

Indicates that the stdin of the container should be kept open.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ImageObject
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Isolation

Specifies whether the container should be run as a Hyper-V Container. Options are HyperV, None, or Default.

```yaml
Type: Docker.PowerShell.Objects.IsolationType
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ImageObject
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

Specifies the name of the newly created container.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ImageObject
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PassThru

Passes the object through the pipeline.

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

### -Publish

The port mappings to publish, in the same `host:container` form the docker CLI takes, for example "8080:80".

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ImageObject
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RemoveAutomatically

Automatically removes the container after running.

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

### -Terminal

Enables terminal emulation in the new container.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Default
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ImageObject
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

### System.String[]

Docker.DotNet.Models.Config Docker.DotNet.Models.ImagesListResponse[]

## OUTPUTS

### Docker.DotNet.Models.ContainerListResponse

## NOTES

## RELATED LINKS

