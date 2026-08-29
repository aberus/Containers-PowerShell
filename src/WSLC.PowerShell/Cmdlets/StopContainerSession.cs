using System.Management.Automation;
using System.Threading.Tasks;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Stop, "ContainerSession",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class StopContainerSession : WslcCmdlet
{
    /// <summary>
    /// The names of the sessions to terminate. When omitted, the default session (or
    /// the one selected by the common Session/SessionName parameters) is terminated.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0)]
    [ValidateNotNullOrEmpty]
    public string[]? Name { get; set; }

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        if (Name is { Length: > 0 })
        {
            foreach (var name in Name)
            {
                Terminate(name);
            }
        }
        else if (Session is not null)
        {
            var name = WslcRuntime.GetSessionName(Session);
            if (ShouldProcess(name, "Terminate session"))
            {
                Session.Terminate();
                WslcRuntime.RemoveSession(name, out _);
            }
        }
        else
        {
            // Like `wslc session terminate`, this only opens an existing session —
            // it never creates the default session just to tear it down.
            Terminate(SessionName ?? WslcRuntime.DefaultSessionName);
        }

        return Task.CompletedTask;
    }

    private void Terminate(string name)
    {
        if (!ShouldProcess(name, "Terminate session"))
        {
            return;
        }

        WslcRuntime.GetSession(name).Terminate();
        WslcRuntime.RemoveSession(name, out _);
    }

    #endregion
}
