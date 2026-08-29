---
document type: cmdlet
external help file: Docker.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/Docker.PowerShell/Help/Submit-ContainerImage.md
Locale: en-US
Module Name: Docker
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Submit-ContainerImage
---

# Submit-ContainerImage

## SYNOPSIS

Submits the container image by pushing it to a Docker registry. Aliased as "Push-ContainerImage".

## SYNTAX

### Default (Default)

```
Submit-ContainerImage [-ImageIdOrName] <string> [-PassThru] [-Authorization <AuthConfig>] [-Force]
 [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ImageObject

```
Submit-ContainerImage [-Image] <ImagesListResponse> [-PassThru] [-Authorization <AuthConfig>]
 [-Force] [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

## ALIASES

Push-ContainerImage

## DESCRIPTION

Submits the container image by pushing it to a Docker registry. Aliased as "Push-ContainerImage".

## EXAMPLES

### Example 1

Pushes the image named "myImage" to the Docker registry.

```powershell
PS C:\> Submit-ContainerImage -ImageIdOrName myImage
```

## PARAMETERS

### -Authorization

A Docker.DotNet.Models.AuthConfig object containing authentication information for the connection to the Docker registry being pushed to.

```yaml
Type: Docker.DotNet.Models.AuthConfig
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

### -CertificateLocation

The location of the X509 certificate file named "key.pfx" that will be used for authentication with the server.  (Note that certificate authorization work is still in progress and this is likely to change).

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

### -Force

Pushes without asking for confirmation that the image should be published to its registry.

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

### -Image

Specifies the container image object to be pushed.

```yaml
Type: Docker.DotNet.Models.ImagesListResponse
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

The Id or Name of the image to be pushed.

```yaml
Type: System.String
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

### -PassThru

If provided, the cmdlet will return the container image object that was submitted.

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

Docker.DotNet.Models.ImagesListResponse

## OUTPUTS

### Docker.DotNet.Models.ContainerListResponse

## NOTES

## RELATED LINKS

