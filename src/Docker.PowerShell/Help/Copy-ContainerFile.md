---
document type: cmdlet
external help file: Docker.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/Docker.PowerShell/Help/Copy-ContainerFile.md
Locale: en-US
Module Name: Docker
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Copy-ContainerFile
---

# Copy-ContainerFile

## SYNOPSIS

Copies a file between container and host.

## SYNTAX

### Default (Default)

```
Copy-ContainerFile [-ContainerIdOrName] <string> [-Path] <string[]> [-Destination <string>]
 [-ToContainer] [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ContainerObject

```
Copy-ContainerFile [-Container] <ContainerListResponse> [-Path] <string[]> [-Destination <string>]
 [-ToContainer] [-HostAddress <string>] [-Context <string>] [-CertificateLocation <string>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Copies a file between container and host.

## EXAMPLES

### Example 1

Copies a file located at $filepathInsideContainer out of the container to the folder "c:\test\" on the host.

```powershell
PS C:\> Copy-ContainerFile -ContainerIdOrName myContainer -Path $filepathInsideContainer -Destination c:\test\
```

### Example 2

Copies a file located at $filepathOnHost on the host to the folder "c:\test\" in the container.

```powershell
PS C:\> Copy-ContainerFile -ContainerIdOrName myContainer -ToContainer -Path $filepathOnHost -Destination c:\test\
```

## PARAMETERS

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

### -Container

Specifies the container.

```yaml
Type: Docker.DotNet.Models.ContainerListResponse
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

Specifies the container by Id or Name.

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

### -Destination

Destination folder for the file.

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

### -Path

The path of the file to copy.

```yaml
Type: System.String[]
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

### -ToContainer

Specifies that the file will be copied from the host into the container. Otherwise, the file is copied from the container to the host.

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

Docker.DotNet.Models.ContainerListResponse

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

