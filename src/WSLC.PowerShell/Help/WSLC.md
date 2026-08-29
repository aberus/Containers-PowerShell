---
document type: module
Help Version: 1.0.0.0
HelpInfoUri: 
Locale: en-US
Module Guid: 278c74c6-9601-4c73-b3d4-90391b95ce10
Module Name: WSLC
ms.date: 08/29/2026
PlatyPS schema version: 2024-05-01
title: WSLC Module
---

# WSLC Module

## Description

This package contains PowerShell cmdlets for Microsoft.WSL.Containers.

The commands are listed here under the names they are declared with. Every one is exported
with the `Wslc` noun prefix from the module manifest's `DefaultCommandPrefix`, so the module
can be loaded alongside Docker.PowerShell, which exports the same names: `New-Container` is
invoked as `New-WslcContainer`. Import with `Import-Module WSLC -Prefix <other>` to choose a
different prefix.

## WSLC

### [Add-ContainerImageTag](Add-ContainerImageTag.md)

Adds a repository and tag to the given image. Aliased as "Tag-ContainerImage".

### [Connect-ContainerRegistry](Connect-ContainerRegistry.md)

Authenticates with a container registry. Aliased as "Login-ContainerRegistry".

### [Disconnect-ContainerRegistry](Disconnect-ContainerRegistry.md)

Forgets the stored token for a container registry. Aliased as "Logout-ContainerRegistry".

### [Get-Container](Get-Container.md)

Returns a list of containers.

### [Get-ContainerComponent](Get-ContainerComponent.md)

Returns the WSLC components that are missing.

### [Get-ContainerDetail](Get-ContainerDetail.md)

Returns the low-level configuration and state of a container.

### [Get-ContainerImage](Get-ContainerImage.md)

Returns a list of container images.

### [Get-ContainerSession](Get-ContainerSession.md)

Returns the WSLC sessions tracked in this PowerShell process.

### [Get-ContainerVersion](Get-ContainerVersion.md)

Returns the version of the WSLC service and SDK.

### [Import-ContainerImage](Import-ContainerImage.md)

Loads a saved image archive, or imports a root filesystem. Aliased as "Load-ContainerImage".

### [Install-ContainerFeature](Install-ContainerFeature.md)

Installs the Windows components WSLC depends on.

### [Invoke-ContainerImage](Invoke-ContainerImage.md)

Creates a container from an image and starts it. Aliased as "Run-ContainerImage", "Run-Container".

### [New-Container](New-Container.md)

Creates a new container from an image.

### [New-ContainerSession](New-ContainerSession.md)

Creates a WSLC session, or returns the one already registered under that name.

### [New-ContainerVolume](New-ContainerVolume.md)

Creates a named VHD volume that containers can mount.

### [Remove-Container](Remove-Container.md)

Removes a container.

### [Remove-ContainerImage](Remove-ContainerImage.md)

Removes a container image.

### [Remove-ContainerVolume](Remove-ContainerVolume.md)

Deletes a named VHD volume and everything stored in it.

### [Request-ContainerImage](Request-ContainerImage.md)

Pulls an image from a registry. Aliased as "Pull-ContainerImage".

### [Start-Container](Start-Container.md)

Starts a container.

### [Start-ContainerProcess](Start-ContainerProcess.md)

Runs a command inside a running container. Aliased as "Exec-Container".

### [Start-ContainerSession](Start-ContainerSession.md)

Starts a WSLC session.

### [Stop-Container](Stop-Container.md)

Stops a running container.

### [Stop-ContainerSession](Stop-ContainerSession.md)

Terminates a WSLC session and the virtual machine backing it.

### [Submit-ContainerImage](Submit-ContainerImage.md)

Pushes an image to its registry. Aliased as "Push-ContainerImage".

### [Wait-Container](Wait-Container.md)

Waits for a container's init process to exit.
