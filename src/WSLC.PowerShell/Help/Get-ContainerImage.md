---
document type: cmdlet
external help file: WSLC.PowerShell.dll-Help.xml
HelpUri: https://github.com/aberus/Containers-PowerShell/blob/master/src/WSLC.PowerShell/Help/Get-ContainerImage.md
Locale: en-US
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Get-ContainerImage
---

# Get-ContainerImage

## SYNOPSIS

Returns a list of container images.

## SYNTAX

### Default (Default)

```
Get-ContainerImage [[-ImageIdOrName] <string[]>] [-Session <Session>] [-SessionName <string>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Returns the images stored in the session, optionally filtered to the given names or id prefixes.

## EXAMPLES

### Example 1

Lists every image in the session

```powershell
PS C:\> Get-WslcContainerImage
```

### Example 2

Gets one image by name

```powershell
PS C:\> Get-WslcContainerImage ubuntu:24.04
```

## PARAMETERS

### -ImageIdOrName

The names or ids of the images to get. When omitted, every image in the session is returned.

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

### Microsoft.WSL.Containers.ImageInfo

## NOTES

## RELATED LINKS

