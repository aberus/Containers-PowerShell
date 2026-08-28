using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Get, "ContainerComponent")]
[OutputType(typeof(Component))]
public sealed class GetContainerComponent : WslcCmdlet
{
    protected override Task ProcessRecordAsync()
    {
        WriteObject(WslcService.GetMissingComponents(), enumerateCollection: true);
        return Task.CompletedTask;
    }
}