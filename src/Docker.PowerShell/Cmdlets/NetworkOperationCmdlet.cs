using System.Management.Automation;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Base class for cmdlets that act on one or more networks, named either by string or by
/// object.
/// </summary>
public abstract class NetworkOperationCmdlet : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The ids of the networks to act on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(NetworkArgumentCompleter))]
    public string[] Id { get; set; }

    /// <summary>
    /// The network objects to act on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.NetworkObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public NetworkResponse[] Network { get; set; }

    #endregion
}
