---
external help file: Docker.PowerShell.dll-Help.xml
online version: 
schema: 2.0.0
---

# Remove-ContainerVolume
## SYNOPSIS
Removes one or more volumes.
## SYNTAX

### Default
```
Remove-ContainerVolume [-Name] <String[]> [-Force] [-HostAddress <String>] [-Context <String>]
 [-CertificateLocation <String>] [<CommonParameters>]
```

### VolumeObject
```
Remove-ContainerVolume [-Volume] <VolumeResponse[]> [-Force] [-HostAddress <String>] [-Context <String>]
 [-CertificateLocation <String>] [<CommonParameters>]
```

## DESCRIPTION
Removes one or more volumes, specified either by name or as the volume objects returned by
Get-ContainerVolume.  The daemon refuses to remove a volume that is still in use by a
container.
## EXAMPLES

### Example 1
```
PS C:\> Remove-ContainerVolume -Name db-data
```

Removes the volume named "db-data".
### Example 2
```
PS C:\> Get-ContainerVolume | Where-Object Driver -eq local | Remove-ContainerVolume
```

Removes every volume that uses the "local" driver.
### Example 3
```
PS C:\> Remove-ContainerVolume -Name db-data -Force
```

Removes the volume named "db-data", succeeding even if that volume no longer exists.
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

### -Force
Forces the removal of the volume.  As with `docker volume rm --force`, a volume that is already gone is reported as success instead of an error.

```yaml
Type: SwitchParameter
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
The names of the volumes to remove.

```yaml
Type: String[]
Parameter Sets: Default
Aliases: 

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Volume
The volume objects to remove, as returned by Get-ContainerVolume.

```yaml
Type: VolumeResponse[]
Parameter Sets: VolumeObject
Aliases: 

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).
## INPUTS

### System.String[]

### Docker.DotNet.Models.VolumeResponse[]

## OUTPUTS

### None

## NOTES
The docker API also accepts a force flag on the remove request itself.  Docker.DotNet 4.3.3 drops that argument before building the request, so -Force is applied by this cmdlet instead.
## RELATED LINKS

[Online Version:](https://github.com/Microsoft/Docker-PowerShell/blob/master/src/Docker.PowerShell/Help/Remove-ContainerVolume.md)
