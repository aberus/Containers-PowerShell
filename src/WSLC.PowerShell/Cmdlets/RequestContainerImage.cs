using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Request, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ImageInfo))]
[Alias("Pull-ContainerImage")]
public class RequestContainerImage : WslcCmdlet
{
    #region Parameters

    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
        Mandatory = true,
        Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Repository { get; set; } = string.Empty;

    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        Position = 1)]
    [ValidateNotNullOrEmpty]
    public string? Tag { get; set; }

    /// <summary>
    /// The registry authentication for the pull. Defaults to the token stored by
    /// Connect-ContainerRegistry for that registry.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? RegistryAuth { get; set; }

    #endregion

    #region Overrides
    protected override async Task ProcessRecordAsync()
    {
        var uri = Tag is null ? Repository : $"{Repository}:{Tag}";
        if (!ShouldProcess(uri, "Pull image"))
        {
            return;
        }

        var options = new PullImageOptions(uri);
        var registryAuth = RegistryAuth ?? RegistryAuthStore.Find(WslcSessionName, Repository);
        if (registryAuth is not null)
        {
            options.RegistryAuth = registryAuth;
        }

        var operation = WslcSession.PullImageAsync(options);
        var progressId = GetNextProgressId();
        operation.Progress = CreateImageProgressHandler(progressId, $"Pulling {uri}");
        await operation;
        CompleteProgress(progressId, $"Pulling {uri}", "Completed");

        var image = WslcSession.GetImages().FirstOrDefault(i =>
            ParameterResolvers.ImageMatches(i, uri) || ParameterResolvers.ImageMatches(i, Repository));
        if (image is not null)
        {
            WriteObject(image);
        }
    }

    #endregion
}
