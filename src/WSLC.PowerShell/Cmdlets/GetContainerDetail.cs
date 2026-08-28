using System.Management.Automation;
using System.Threading.Tasks;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Get, "ContainerDetail",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class GetContainerDetail : MultiContainerOperationCmdlet
{
    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        foreach (var container in ParameterResolvers.GetContainers(Container, ContainerIdOrName, () => WslcSession))
        {
            WriteObject(JsonConversion.ToPSObject(container.Inspect()));
        }

        return Task.CompletedTask;
    }

    #endregion
}
