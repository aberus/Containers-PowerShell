using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Get, "ContainerVersion")]
[OutputType(typeof(ServiceVersion))]
public sealed class GetContainerVersion : WslcCmdlet
{
    protected override Task ProcessRecordAsync()
    {
        WriteObject(WslcService.GetVersion());
        return Task.CompletedTask;
    }
}