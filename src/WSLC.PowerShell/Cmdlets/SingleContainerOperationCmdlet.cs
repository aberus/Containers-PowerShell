using System.Management.Automation;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Cmdlets;

public abstract class SingleContainerOperationCmdlet : WslcCmdlet
{
    #region Parameters

    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(ContainerArgumentCompleter))]
    [Alias("Name", "Id")]
    public string? ContainerIdOrName { get; set; }

    [Parameter(ParameterSetName = CommonParameterSetNames.ContainerObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNull]
    public Container? Container { get; set; }

    #endregion
}
