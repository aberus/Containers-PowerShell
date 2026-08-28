using System.Management.Automation;
using System.Threading.Tasks;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommunications.Disconnect, "ContainerRegistry",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Logout-ContainerRegistry")]
public sealed class DisconnectContainerRegistry : WslcCmdlet
{
    #region Parameters

    /// <summary>
    /// The registry to forget the stored token for. Defaults to Docker Hub.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
        Position = 0)]
    [ValidateNotNullOrEmpty]
    [Alias("ServerAddress", "Registry")]
    public string Server { get; set; } = RegistryAuthStore.DefaultServerAddress;

    /// <summary>
    /// Forgets the stored tokens for every registry in the session.
    /// </summary>
    [Parameter(ParameterSetName = AllParameterSetName, Mandatory = true)]
    public SwitchParameter All { get; set; }

    #endregion

    private const string AllParameterSetName = "All";

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        var sessionName = WslcSessionName;

        if (All)
        {
            if (ShouldProcess(sessionName, "Forget all stored container registry tokens"))
            {
                var removed = RegistryAuthStore.RemoveAll(sessionName);
                WriteVerbose($"Forgot {removed} stored container registry token(s).");
            }

            return Task.CompletedTask;
        }

        var serverUri = RegistryAuthStore.ToServerUri(Server);
        if (ShouldProcess(serverUri.ToString(), "Forget stored container registry token") &&
            !RegistryAuthStore.Remove(sessionName, Server))
        {
            WriteVerbose($"No stored token was held for '{serverUri}'.");
        }

        return Task.CompletedTask;
    }

    #endregion
}
