using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using Windows.Foundation;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

//https://wsl.dev/api-reference/csharp/overview/

/// <summary>
/// Contains strings commonly used as parameter set names.
/// </summary>
internal static class CommonParameterSetNames
{
    public const string Default = "Default";
    public const string ContainerObject = "ContainerObject";
    public const string ImageObject = "ImageObject";
    public const string SessionObject = "SessionObject";
}

public abstract class WslcCmdlet : PSCmdlet
{
    #region Private members

    private readonly CancellationTokenSource cancelSignal = new();
    private int nextProgressId;
    private Session? wslcSession;

    protected CancellationToken CmdletCancellationToken => cancelSignal.Token;

    /// <summary>
    /// The WSLC session to operate on. Resolved from the Session/SessionName common
    /// parameters; when neither is given, the default session is created/reused.
    /// </summary>
    protected Session WslcSession => wslcSession ??= WslcRuntime.ResolveSession(Session, SessionName);

    /// <summary>
    /// The session's name, resolved without creating or starting the default session —
    /// for cmdlets that only key process-local state by session.
    /// </summary>
    protected string WslcSessionName => Session is not null || SessionName is not null
        ? WslcRuntime.GetSessionName(WslcSession)
        : WslcRuntime.DefaultSessionName;

    #endregion

    #region Parameters

    /// <summary>
    /// The common parameter for specifying the WSLC session object to operate on.
    /// </summary>
    [Parameter]
    [ValidateNotNull]
    public Session? Session { get; set; }

    /// <summary>
    /// The common parameter for specifying the name of the WSLC session to operate on.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? SessionName { get; set; }

    #endregion

    #region Overrides

    protected sealed override void ProcessRecord()
    {
        try
        {
            AsyncPump.Run(ProcessRecordAsync);
        }
        catch (Exception e) when (e is not PipelineStoppedException) // PipelineStoppedException shouldn't be ignored.
        {
            // Handle the exception and continue to process other objects.
            WriteError(new ErrorRecord(e, "WSLC Client Exception", ErrorCategory.NotSpecified, null));
        }
    }

    /// <summary>
    /// Common StopProcessing code, that signals the CancellationToken. This may or may
    /// not be used by child classes in calls to WSLC.
    /// </summary>
    protected override void StopProcessing()
    {
        AsyncPump.Run(StopProcessingAsync);

        cancelSignal.Cancel();
    }

    protected abstract Task ProcessRecordAsync();

    protected virtual Task StopProcessingAsync()
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Progress reporting

    protected int GetNextProgressId()
    {
        return Interlocked.Increment(ref nextProgressId);
    }

    /// <summary>
    /// Creates a progress handler for an image operation that marshals reporting back
    /// to the pipeline thread via the cmdlet's synchronization context.
    /// </summary>
    protected AsyncActionProgressHandler<ImageProgress> CreateImageProgressHandler(int id, string activity)
    {
        IProgress<ImageProgress> progress = new Progress<ImageProgress>(p => ReportImageProgress(id, activity, p));
        return (_, p) => progress.Report(p);
    }

    /// <summary>
    /// Creates a progress handler for an install operation that marshals reporting back
    /// to the pipeline thread via the cmdlet's synchronization context.
    /// </summary>
    protected AsyncActionProgressHandler<InstallProgress> CreateInstallProgressHandler(int id, string activity)
    {
        IProgress<InstallProgress> progress = new Progress<InstallProgress>(p => ReportInstallProgress(id, activity, p));
        return (_, p) => progress.Report(p);
    }

    protected void ReportImageProgress(int id, string activity, ImageProgress progress)
    {
        var record = new ProgressRecord(id, activity, progress.Status.ToString())
        {
            PercentComplete = GetPercentComplete(progress.CurrentBytes, progress.TotalBytes)
        };

        if (progress.TotalBytes > 0)
        {
            record.CurrentOperation = $"{progress.CurrentBytes}/{progress.TotalBytes} bytes";
        }

        WriteProgress(record);
    }

    protected void ReportInstallProgress(int id, string activity, InstallProgress progress)
    {
        var record = new ProgressRecord(id, activity, progress.Component.ToString())
        {
            PercentComplete = GetPercentComplete(progress.Progress, progress.Total)
        };

        if (progress.Total > 0)
        {
            record.CurrentOperation = $"{progress.Progress}/{progress.Total}";
        }

        WriteProgress(record);
    }

    protected void CompleteProgress(int id, string activity, string statusDescription)
    {
        var record = new ProgressRecord(id, activity, statusDescription)
        {
            RecordType = ProgressRecordType.Completed
        };

        WriteProgress(record);
    }

    private static int GetPercentComplete(ulong current, ulong total)
    {
        if (total == 0)
        {
            return -1;
        }

        return (int)Math.Clamp(current * 100 / total, 0, 100);
    }

    private static int GetPercentComplete(uint current, uint total)
    {
        if (total == 0)
        {
            return -1;
        }

        return (int)Math.Clamp((ulong)current * 100 / total, 0, 100);
    }

    #endregion
}
