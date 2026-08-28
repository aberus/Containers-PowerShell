using System.Management.Automation;
using Docker.DotNet.Models;
using System.Collections.Generic;
using Docker.PowerShell.Objects;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "ContainerNet",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(NetworkResponse))]
public class NewContainerNet : DkrCmdlet
{
    #region Parameters

    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Name;

    [Parameter(Position = 1)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(NetworkArgumentCompleter))]
    public string Driver;

    [Parameter]
    public SwitchParameter Internal;

    [Parameter]
    public SwitchParameter EnableIPv6;

    [Parameter]
    public IPAM IPAM;

    [Parameter]
    public IDictionary<string, string> Options;

    [Parameter]
    public IDictionary<string, string> Labels;

    #endregion

    #region Overrides

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