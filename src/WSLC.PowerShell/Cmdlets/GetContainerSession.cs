using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Get, "ContainerSession",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Session))]
public class GetContainerSession : WslcCmdlet
{
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0)]
    [ValidateNotNullOrEmpty]
    public string[]? Name { get; set; }

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        if (Name is null || Name.Length == 0)
        {
            WriteObject(WslcRuntime.GetSessions(), enumerateCollection: true);
            return Task.CompletedTask;
        }

        foreach (var name in Name)
        {
            WriteObject(WslcRuntime.GetSession(name));
        }

        return Task.CompletedTask;
    }

    #endregion
}
