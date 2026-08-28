using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Renders the daemon's stream of JSON status messages as PowerShell progress bars and
/// host output.
/// </summary>
internal class JsonMessageWriter
{
    /// <summary>
    /// Creates a writer that reports through the given cmdlet.
    /// </summary>
    public JsonMessageWriter(PSCmdlet cmdlet)
    {
        _cmdlet = cmdlet;
    }

    /// <summary>
    /// Reports one message from the daemon.
    /// </summary>
    public void WriteJsonMessage(JSONMessage message)
    {
        if (message.Error != null)
        {
            var error = new ErrorRecord(new Exception(message.Error.Message), null, ErrorCategory.OperationStopped, null);
            _cmdlet.WriteError(error);
        }
        else if (message.Progress != null)
        {
            var id = message.ID ?? "";
            int activity;
            if (!_idToActivity.TryGetValue(id, out activity))
            {
                activity = _nextActivity;
                _nextActivity++;
                _idToActivity.Add(id, activity);
            }

            var activityName = new StringBuilder(id);
            if (activityName.Length == 0)
            {
                activityName.Append("Operation");
            }

            var record = new ProgressRecord(activity, activityName.ToString(), message.Status ?? "Processing");

            var progress = message.Progress;
            if (progress.Total > 0 && progress.Current <= progress.Total)
            {
                record.PercentComplete = (int)(progress.Current * 100 / progress.Total);
            }

            if (progress.Current > 0)
            {
                record.CurrentOperation = string.Format(" ({0} bytes)", progress.Current);
            }

            _cmdlet.WriteProgress(record);
        }
        else
        {
            var info = new StringBuilder();
            if (message.ID != null)
            {
                info.Append(message.ID);
                info.Append(": ");
            }

            var infoRecord = new HostInformationMessage();
            if (message.Stream != null)
            {
                info.Append(message.Stream);
                infoRecord.NoNewLine = true;
            }
            else
            {
                info.Append(message.Status);
            }

            infoRecord.Message = info.ToString();
            _cmdlet.WriteInformation(infoRecord, ["PSHOST"]);
        }
    }

    /// <summary>
    /// Completes any progress bars still on screen.
    /// </summary>
    public void ClearProgress()
    {
        foreach (var activity in _idToActivity)
        {
            var record = new ProgressRecord(activity.Value, "Operation", "Processing");
            record.RecordType = ProgressRecordType.Completed;
            _cmdlet.WriteProgress(record);
        }
    }

    private Dictionary<string, int> _idToActivity = new Dictionary<string, int>();
    private int _nextActivity = 0x10000;
    private PSCmdlet _cmdlet;
}