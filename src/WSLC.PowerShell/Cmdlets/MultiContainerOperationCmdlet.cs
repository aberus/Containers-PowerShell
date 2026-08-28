using System.Management.Automation;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Cmdlets;

public abstract class MultiContainerOperationCmdlet : WslcCmdlet
{
    #region Parameters

    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [Alias("Name", "Id")]
    public string[]? ContainerIdOrName { get; set; }

    [Parameter(ParameterSetName = CommonParameterSetNames.ContainerObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public Container[]? Container { get; set; }

    #endregion
}
