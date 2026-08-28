using System.Management.Automation;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Base class for cmdlets that act on a single container, named either by string or by
/// object.
/// </summary>
public abstract class SingleContainerOperationCmdlet : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The name or id of the container to act on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(ContainerArgumentCompleter))]
    [Alias("Name", "Id")]
    public string ContainerIdOrName { get; set; }

    /// <summary>
    /// The container object to act on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.ContainerObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public ContainerListResponse Container { get; set; }

    #endregion
}
