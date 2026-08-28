using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Completes volume names by asking the daemon what exists.
/// </summary>
public class VolumeArgumentCompleter : IArgumentCompleter
{
    /// <summary>
    /// The drivers docker ships with, so that -Driver still completes on a daemon that
    /// has no volume using them yet.
    /// </summary>
    private static readonly string[] BuiltInDrivers = ["local"];

    /// <summary>
    /// Returns the completions that start with the text typed so far.
    /// </summary>
    public IEnumerable<CompletionResult> CompleteArgument(string commandName,
                                                          string parameterName,
                                                          string wordToComplete,
                                                          CommandAst commandAst,
                                                          IDictionary fakeBoundParameters)
    {
        wordToComplete ??= string.Empty;

        var client = DockerFactory.CreateClient(fakeBoundParameters);

        // The daemon reports no volumes as a null list rather than an empty one.
        IList<VolumeResponse> volumes = client.Volumes.ListAsync(new VolumesListParameters()).GetAwaiter().GetResult().Volumes ?? [];

        // Volumes are addressed by name, so every parameter other than the ones describing
        // the volume itself completes to a name.
        IEnumerable<string> candidates = parameterName switch
        {
            "Driver" => volumes.Select(volume => volume.Driver).Concat(BuiltInDrivers),
            "Scope" => volumes.Select(volume => volume.Scope),
            _ => volumes.Select(volume => volume.Name),
        };

        return candidates
            .Where(value => !string.IsNullOrEmpty(value))
            .Distinct()
            .Where(value => value.StartsWith(wordToComplete, StringComparison.CurrentCultureIgnoreCase))
            .OrderBy(value => value)
            .Select(value => new CompletionResult(
                value.Contains(" ") ? "\"" + value + "\"" : value,
                value,
                CompletionResultType.Text,
                value));
    }
}
