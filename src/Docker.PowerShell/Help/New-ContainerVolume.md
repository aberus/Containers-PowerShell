---
external help file: Docker.PowerShell.dll-Help.xml
online version: 
schema: 2.0.0
---

# New-ContainerVolume
## SYNOPSIS
Creates a new volume.
## SYNTAX

```
New-ContainerVolume [-HostAddress <String>] [-Context <String>] [-CertificateLocation <String>]
 [[-Name] <String>] [[-Driver] <String>]
 [-Options <System.Collections.Generic.IDictionary`2[System.String,System.String]>]
 [-Labels <System.Collections.Generic.IDictionary`2[System.String,System.String]>] [<CommonParameters>]
```

## DESCRIPTION
Creates a new volume and writes the created volume to the pipeline.
## EXAMPLES

### Example 1
```
PS C:\> New-ContainerVolume -Name db-data
```

Creates a new volume called "db-data" using the daemon's default driver.
### Example 2
```
PS C:\> New-ContainerVolume
```

Creates an anonymous volume, letting the daemon generate its name.
### Example 3
```
PS C:\> $opt = New-Object 'System.Collections.Generic.Dictionary[String,String]'
PS C:\> $opt.add("type","tmpfs")
PS C:\> $opt.add("device","tmpfs")
PS C:\> New-ContainerVolume -Name scratch -Driver local -Options $opt
```

Creates a tmpfs-backed volume called "scratch" using the "local" driver.
## PARAMETERS

### -CertificateLocation
The location of the X509 certificate file named "key.pfx" that will be used for authentication with the server. (Note that certificate authorization work is still in progress and this is likely to change).

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Context
The name of the docker context to connect through. The context supplies the endpoint and any TLS material.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Driver
The name of the volume driver plugin to use.  If not specified, uses the default configured on the daemon.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HostAddress
The address of the docker daemon to connect to.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Labels
A dictionary containing labels to set on the volume.

```yaml
Type: System.Collections.Generic.IDictionary`2[System.String,System.String]
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
The volume name to use.  If not specified, the daemon generates one.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Options
A dictionary containing driver specific volume options.

```yaml
Type: System.Collections.Generic.IDictionary`2[System.String,System.String]
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).
## INPUTS

### None

## OUTPUTS

### Docker.DotNet.Models.VolumeResponse

## NOTES

## RELATED LINKS

[Online Version:](https://github.com/Microsoft/Docker-PowerShell/blob/master/src/Docker.PowerShell/Help/New-ContainerVolume.md)
