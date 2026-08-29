---
document type: cmdlet
external help file: Docker.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/Docker.PowerShell/Help/Export-ContainerImage.md
Locale: en-US
Module Name: Docker
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Export-ContainerImage
---

# Export-ContainerImage

## SYNOPSIS

Exports the container image, including all layers, into a single compressed file. Aliased to "Save-ContainerImage".

## SYNTAX

### Default (Default)

```
Export-ContainerImage [-ImageIdOrName] <string[]> [-DestinationFilePath] <string> [-Force]
 [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ImageObject

```
Export-ContainerImage [-Image] <ImagesListResponse[]> [-DestinationFilePath] <string> [-Force]
 [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## ALIASES

Save-ContainerImage

## DESCRIPTION

Exports the container image, including all layers, into a single compressed file. Aliased to "Save-ContainerImage".

## EXAMPLES

### Example 1

Exports all layers for the image "myImage" to the compressed file "c:\myImage.tar".

```powershell
PS C:\> Export-ContainerImage -ImageIdOrName myImage -DestinationFilePath c:\myImage.tar
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

### -DestinationFilePath

The path to export the image to.

```yaml
Type: System.String
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

### -Force

Replaces the destination file without asking first. Without it, an existing file raises a prompt before its contents are overwritten.

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

The image that will be exported to a file.

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

The Name or Id of the image that will be exported.

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

Docker.DotNet.Models.ImagesListResponse[]

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

