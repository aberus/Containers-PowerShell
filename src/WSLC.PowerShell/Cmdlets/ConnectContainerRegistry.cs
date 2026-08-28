using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommunications.Connect, "ContainerRegistry", SupportsShouldProcess = true)]
[OutputType(typeof(AuthenticateResult))]
[Alias("Login-ContainerRegistry")]
public sealed class ConnectContainerRegistry : WslcCmdlet
{
    #region Parameters

    /// <summary>
    /// The registry to authenticate with; a bare host assumes https. Defaults to Docker Hub.
    /// </summary>
    [Parameter(Position = 0, ValueFromPipeline = true)]
    [ValidateNotNullOrEmpty]
    [Alias("ServerAddress", "Registry")]
    public string Server { get; set; } = RegistryAuthStore.DefaultServerAddress;

    /// <summary>
    /// The credential to authenticate with; PowerShell prompts when it is not supplied.
    /// </summary>
    [Parameter(Position = 1, Mandatory = true)]
    [ValidateNotNull]
    [Credential]
    public PSCredential Credential { get; set; } = PSCredential.Empty;

    /// <summary>
    /// If specified, outputs the authentication result. It carries a credential, so it is
    /// withheld by default.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Authenticates and stores the token for later pulls and pushes to that registry.
    /// </summary>
    protected override Task ProcessRecordAsync()
    {
        var serverUri = RegistryAuthStore.ToServerUri(Server);
        if (!ShouldProcess(serverUri.ToString(), "Authenticate with container registry"))
        {
            return Task.CompletedTask;
        }

        var credential = Credential.GetNetworkCredential();
        var result = WslcSession.Authenticate(serverUri, credential.UserName, credential.Password);

        RegistryAuthStore.Set(WslcSessionName, Server, result);

        if (result.TokenType == IdentityTokenType.Credentials)
        {
            WriteVerbose(
                $"'{serverUri}' issued no identity token; the supplied credentials are used for subsequent pulls and pushes.");
        }

        if (PassThru)
        {
            WriteObject(result);
        }

        return Task.CompletedTask;
    }

    #endregion
}
