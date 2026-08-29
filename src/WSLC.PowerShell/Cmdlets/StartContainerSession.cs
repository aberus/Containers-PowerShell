using System;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Start, "ContainerSession",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class StartContainerSession : WslcCmdlet
{
    /// <summary>
    /// The names of the sessions to start. When omitted, the default session is
    /// created and started if needed (like the wslc CLI's open-or-create default),
    /// unless the common Session/SessionName parameters select another session.
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
                EnsureStarted(WslcRuntime.GetSession(name), name);
            }
        }
        else if (Session is not null || SessionName is not null)
        {
            EnsureStarted(WslcSession, WslcRuntime.GetSessionName(WslcSession));
        }
        else
        {
            // Sessions created by this module are started on creation, so this is
            // simply "ensure the default session exists and is running".
            if (ShouldProcess(WslcRuntime.DefaultSessionName, "Start session"))
            {
                WslcRuntime.GetOrCreateDefaultSession();
            }
        }

        return Task.CompletedTask;
    }

    private void EnsureStarted(Session session, string name)
    {
        if (!ShouldProcess(name, "Start session"))
        {
            return;
        }

        try
        {
            session.Start();
        }
        catch (InvalidOperationException)
        {
            // The SDK throws when Start is called on an already started session.
            WriteVerbose($"WSLC session '{name}' is already started.");
        }
    }

    #endregion
}
