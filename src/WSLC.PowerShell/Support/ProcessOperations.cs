using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Support;

internal static class ProcessOperations
{
    /// <summary>
    /// Waits for a WSLC process to exit, honoring the cmdlet cancellation token.
    /// </summary>
    public static async Task<int> WaitForExitAsync(Process process, CancellationToken cancellationToken)
    {
        var exited = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        ProcessExitHandler onExited = code => exited.TrySetResult(code);
        process.Exited += onExited;
        try
        {
            if (process.State is ProcessState.Exited or ProcessState.Signalled)
            {
                return process.ExitCode;
            }

            using (cancellationToken.Register(() => exited.TrySetCanceled(cancellationToken)))
            {
                return await exited.Task.ConfigureAwait(false);
            }
        }
        finally
        {
            process.Exited -= onExited;
        }
    }

    /// <summary>
    /// Subscribes to a WSLC process's output events (marshalled back to the calling
    /// synchronization context), optionally starts it, and waits for it to exit.
    /// The process must have been created with ProcessOutputMode.Event.
    /// </summary>
    public static async Task<int> AttachAndWaitAsync(
        Process process,
        Action<byte[]> onOutput,
        Action<byte[]> onError,
        Action? start,
        CancellationToken cancellationToken)
    {
        // Progress<T> captures the current SynchronizationContext (the cmdlet's
        // AsyncPump), so the callbacks run on the pipeline thread.
        IProgress<byte[]> output = new Progress<byte[]>(onOutput);
        IProgress<byte[]> error = new Progress<byte[]>(onError);
        var exited = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        ProcessOutputHandler onOutputReceived = data => output.Report(data);
        ProcessOutputHandler onErrorReceived = data => error.Report(data);
        ProcessExitHandler onExited = code => exited.TrySetResult(code);

        process.OutputReceived += onOutputReceived;
        process.ErrorReceived += onErrorReceived;
        process.Exited += onExited;
        try
        {
            start?.Invoke();

            if (process.State is ProcessState.Exited or ProcessState.Signalled)
            {
                return process.ExitCode;
            }

            using (cancellationToken.Register(() => exited.TrySetCanceled(cancellationToken)))
            {
                return await exited.Task.ConfigureAwait(false);
            }
        }
        finally
        {
            process.OutputReceived -= onOutputReceived;
            process.ErrorReceived -= onErrorReceived;
            process.Exited -= onExited;
        }
    }
}
