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

/// <summary>
/// Base class for this module's cmdlets. Supplies the parameters that choose a daemon, a
/// client built on first use, and an async ProcessRecord that runs on the pipeline thread.
/// </summary>
public abstract class DkrCmdlet : PSCmdlet
{
    #region Private members

    private CancellationTokenSource cancelSignal = new CancellationTokenSource();

    /// <summary>
    /// Signalled when the pipeline stops, so calls to the daemon can be abandoned.
    /// </summary>
    protected CancellationToken CmdletCancellationToken => cancelSignal.Token;

    /// <summary>
    /// The client for the daemon named by the connection parameters, built on first use.
    /// </summary>
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

    #region Confirmation

    /// <summary>
    /// The "Yes to All" / "No to All" answers to <c>ShouldContinue</c>. They live on
    /// the cmdlet rather than at the call site so that an answer given for one pipeline item
    /// still applies to the rest of the invocation.
    /// </summary>
    private bool yesToAll;

    private bool noToAll;

    /// <summary>
    /// Confirms a change that no other cmdlet in this module can undo: data deleted with a
    /// volume, an image published to a registry, a file overwritten on disk. ShouldProcess
    /// runs first, so -WhatIf and -Confirm behave as they do everywhere else; the extra
    /// ShouldContinue prompt then asks about the consequence itself, and only -Force answers
    /// that one in advance.
    /// </summary>
    /// <param name="target">The resource being changed, as reported by -WhatIf.</param>
    /// <param name="action">The change being made, also used as the prompt caption.</param>
    /// <param name="query">The question describing what cannot be taken back.</param>
    /// <param name="force">Whether -Force was supplied, which answers the query with yes.</param>
    /// <returns>True when the caller should go ahead with the change.</returns>
    protected bool ShouldProcessAndContinue(string target, string action, string query, bool force)
    {
        if (!ShouldProcess(target, action))
        {
            return false;
        }

        return force || ShouldContinue(query, action, ref yesToAll, ref noToAll);
    }

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

    /// <summary>
    /// Runs <see cref="ProcessRecordAsync"/> on the pipeline thread and reports anything it
    /// throws as an error, so that one bad input does not end the whole pipeline.
    /// </summary>
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

    /// <summary>
    /// Does the cmdlet's work for one pipeline input.
    /// </summary>
    protected abstract Task ProcessRecordAsync();

    /// <summary>
    /// Cleans up when the pipeline stops. Overridden by cmdlets that leave something running.
    /// </summary>
    protected virtual Task StopProcessingAsync()
    {
        return Task.CompletedTask;
    }

    #endregion
}
