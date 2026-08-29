---
document type: module
Help Version: 1.0.0.0
HelpInfoUri: 
Locale: en-US
Module Guid: 7cc6f829-b4b5-493d-9a99-f92dc54d7e10
Module Name: Docker
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: Docker Module
---

# Docker Module

## Description

This package contains Docker PowerShell cmdlets that can be used to interact with Windows and Linux Docker hosts.

## Docker

### [Add-ContainerImageTag](Add-ContainerImageTag.md)

Adds a repository and tag to the given image. Aliased as "Tag-ContainerImage".

### [ConvertTo-ContainerImage](ConvertTo-ContainerImage.md)

Creates a new container image by committing an existing container. Aliased as "Commit-Container".

### [Copy-ContainerFile](Copy-ContainerFile.md)

Copies a file between container and host.

### [Enter-ContainerSession](Enter-ContainerSession.md)

Connects to interactive session in the specified container. Aliased as "Attach-Container".

### [Export-ContainerImage](Export-ContainerImage.md)

Exports the container image, including all layers, into a single compressed file. Aliased to "Save-ContainerImage".

### [Get-Container](Get-Container.md)

Returns a list of containers.

### [Get-ContainerDetail](Get-ContainerDetail.md)

Gets details about a container.

### [Get-ContainerImage](Get-ContainerImage.md)

Returns container images.

### [Get-ContainerNet](Get-ContainerNet.md)

Gets a network endpoint.

### [Get-ContainerNetDetail](Get-ContainerNetDetail.md)

Gets details about a network endpoint.

### [Get-ContainerVolume](Get-ContainerVolume.md)

Gets the volumes on the daemon.

### [Import-ContainerImage](Import-ContainerImage.md)

Imports the container image, including all layers, from a single compressed file. Aliased to "Load-ContainerImage".

### [Invoke-ContainerImage](Invoke-ContainerImage.md)

Runs a container from an existing image. Aliased as "Run-Container".

### [New-Container](New-Container.md)

Creates a new container.

### [New-ContainerImage](New-ContainerImage.md)

Builds a new container image from a set of instructions in a Dockerfile. Aliased as "Build-ContainerImage".

### [New-ContainerNet](New-ContainerNet.md)

Creates a new network.

### [New-ContainerVolume](New-ContainerVolume.md)

Creates a new volume.

### [Remove-Container](Remove-Container.md)

Removes a container.

### [Remove-ContainerImage](Remove-ContainerImage.md)

Removes a container image.

### [Remove-ContainerNet](Remove-ContainerNet.md)

Removes a network endpoint.

### [Remove-ContainerVolume](Remove-ContainerVolume.md)

Removes one or more volumes.

### [Request-ContainerImage](Request-ContainerImage.md)

Downloads a container image matching the given repository and tag from the Docker registry. Aliased as "Pull-ContainerImage".

### [Start-Container](Start-Container.md)

Starts a container.

### [Start-ContainerProcess](Start-ContainerProcess.md)

Starts a new process with the given command in the specified container. Aliased as "Exec-Container".

### [Stop-Container](Stop-Container.md)

Stops a running container.

### [Submit-ContainerImage](Submit-ContainerImage.md)

Submits the container image by pushing it to a Docker registry. Aliased as "Push-ContainerImage".

### [Wait-Container](Wait-Container.md)

Waits for the given container to shutdown, often indicating that the process run inside the container has completed.

