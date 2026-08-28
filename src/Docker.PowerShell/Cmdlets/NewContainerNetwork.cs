using System.Management.Automation;
using Docker.DotNet.Models;
using System.Collections.Generic;
using Docker.PowerShell.Objects;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Creates a network, as <c>docker network create</c> does.
/// </summary>
[Cmdlet(VerbsCommon.New, "ContainerNet",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(NetworkResponse))]
public class NewContainerNet : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The name to give the network.
    /// </summary>
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Name;

    /// <summary>
    /// The network driver to use. The daemon's default is used when omitted.
    /// </summary>
    [Parameter(Position = 1)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(NetworkArgumentCompleter))]
    public string Driver;

    /// <summary>
    /// Keeps the network isolated from the outside, with no external routing.
    /// </summary>
    [Parameter]
    public SwitchParameter Internal;

    /// <summary>
    /// Enables IPv6 addressing on the network.
    /// </summary>
    [Parameter]
    public SwitchParameter EnableIPv6;

    /// <summary>
    /// The address management configuration, for choosing subnets and gateways.
    /// </summary>
    [Parameter]
    public IPAM IPAM;

    /// <summary>
    /// Driver specific options.
    /// </summary>
    [Parameter]
    public IDictionary<string, string> Options;

    /// <summary>
    /// Labels to set on the network.
    /// </summary>
    [Parameter]
    public IDictionary<string, string> Labels;

    #endregion

    #region Overrides

    /// <summary>
    /// Creates the network and writes it.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        if (!ShouldProcess(Name, "Create network"))
        {
            return;
        }

        var createResult = await DkrClient.Networks.CreateNetworkAsync(new NetworksCreateParameters()
        {
            Name = Name,
            Driver = Driver,
            Internal = Internal,
            EnableIPv6 = EnableIPv6,
            IPAM = IPAM,
            Options = Options,
            Labels = Labels
        });

        if (!string.IsNullOrEmpty(createResult.Warning))
        {
            WriteWarning(createResult.Warning);
        }

        if (!string.IsNullOrEmpty(createResult.ID))
        {
            WriteObject(await NetworkOperations.GetNetworksById(createResult.ID, DkrClient));
        }
    }

    #endregion
}