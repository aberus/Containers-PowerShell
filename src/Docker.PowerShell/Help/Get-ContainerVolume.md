---
external help file: Docker.PowerShell.dll-Help.xml
online version: 
schema: 2.0.0
---

# Get-ContainerVolume
## SYNOPSIS
Gets the volumes on the daemon.
## SYNTAX

```
Get-ContainerVolume [[-Name] <String[]>] [-HostAddress <String>] [-Context <String>]
 [-CertificateLocation <String>] [<CommonParameters>]
```

## DESCRIPTION
Gets the volumes on the daemon.  If no name is specified, every volume is returned.  Any
warnings the daemon reports while listing (for example a volume driver it could not reach)
are written to the warning stream.
## EXAMPLES

### Example 1
```
PS C:\> Get-ContainerVolume
```

Gets every volume on the daemon.
### Example 2
```
PS C:\> Get-ContainerVolume -Name db-data
```

Gets the volume named "db-data".  Names are matched exactly, so this does not also return
a volume named "db-data-backup".
### Example 3
```
PS C:\> Get-ContainerVolume | Where-Object Driver -eq local | Remove-ContainerVolume
```

Removes every volume that uses the "local" driver.
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

### -Name
The names of the volumes to get.  If not specified, every volume is returned.  A name that matches no volume returns nothing rather than an error.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).
## INPUTS

### System.String[]

## OUTPUTS

### Docker.DotNet.Models.VolumeResponse

## NOTES

## RELATED LINKS

[Online Version:](https://github.com/Microsoft/Docker-PowerShell/blob/master/src/Docker.PowerShell/Help/Get-ContainerVolume.md)
