using System.Management.Automation;
using Docker.DotNet;
using System.Threading;
using System.Threading.Tasks;
using Docker.PowerShell.Support;
using System;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Contains strings commonly used as parameter set names.
/// </summary>
internal static class CommonParameterSetNames
{
    public const string Default = "Default";
    public const string ContainerObject = "ContainerObject";
    public const string ImageObject = "ImageObject";
    public const string ConfigObject = "ConfigObject";
    public const string NetworkName = "NetworkName";
    public const string NetworkObject = "NetworkObject";
    public const string VolumeObject = "VolumeObject";
    public const string AllImages = "AllImages";
}

public abstract class DkrCmdlet : PSCmdlet
{
    #region Private members

    private CancellationTokenSource cancelSignal = new CancellationTokenSource();

    protected CancellationToken CmdletCancellationToken => cancelSignal.Token;

    protected DockerClient DkrClient
    {
        get
        {
            return dkrClient ??= DockerFactory.CreateClient(HostAddress, Context, CertificateLocation);
        }
    }

    private DockerClient dkrClient;

    #endregion

    #region Parameters

    /// <summary>
    /// The common parameter for specifying the address of the host to operate on.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string HostAddress { get; set; }

    /// <summary>
    /// The common parameter for selecting a docker context to connect through. The
    /// context supplies the endpoint and any TLS material.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Context { get; set; }

    ///<summary>
    /// The common parameter for specifying the location to find certificates for use in secure
    /// connections.
    ///</summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string CertificateLocation { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Common StopProcessing code, that signals the CancellationToken. This may or may
    /// not be used be child classes in http calls to docker.
    /// </summary>
    protected override void StopProcessing()
    {
        AsyncPump.Run(StopProcessingAsync);

        cancelSignal.Cancel();
    }

    protected sealed override void ProcessRecord()
    {
        try
        {
            AsyncPump.Run(ProcessRecordAsync);
        }
        catch (Exception e) when (e is not PipelineStoppedException) // PipelineStoppedException shouldn't be ignored.
        {
            // Handle the exception and continue to process other objects.
            WriteError(new ErrorRecord(e, "Docker Client Exception", ErrorCategory.NotSpecified, null));
        }
    }

    protected abstract Task ProcessRecordAsync();

    protected virtual Task StopProcessingAsync()
    {
        return Task.CompletedTask;
    }

    #endregion
}
