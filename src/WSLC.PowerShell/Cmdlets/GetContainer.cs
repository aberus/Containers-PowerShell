using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Get, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Container))]
public class GetContainer : WslcCmdlet
{
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0)]
    [ValidateNotNullOrEmpty]
    [Alias("Name", "Id")]
    public string[]? ContainerIdOrName { get; set; }

    #region Overrides
    /// <summary>
    /// Outputs container objects for each container matching the provided parameters.
    /// </summary>
    protected override Task ProcessRecordAsync()
    {
        if (ContainerIdOrName != null)
        {
            foreach (var id in ContainerIdOrName)
            {
                WriteObject(WslcRuntime.GetContainer(id, () => WslcSession));
            }
        }
        else
        {
            // Only filter by session when one was explicitly specified; otherwise list the
            // containers of every session this module has created one in. A session name
            // is used as given rather than resolved through WslcSession, so that listing a
            // session created by an earlier PowerShell process does not fail on it not
            // being registered here.
            var sessionName = Session is not null ? WslcRuntime.GetSessionName(WslcSession) : SessionName;

            // Containers outlive the process that created them, but the SDK cannot
            // enumerate them, so they are opened again from the module's own record.
            WslcRuntime.RestoreContainers(sessionName, WriteWarning);

            WriteObject(WslcRuntime.GetContainers(sessionName), enumerateCollection: true);
        }

        return Task.CompletedTask;
    }

    #endregion
}
