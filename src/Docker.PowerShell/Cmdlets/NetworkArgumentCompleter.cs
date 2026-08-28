using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Completes network names and ids by asking the daemon what exists.
/// </summary>
public class NetworkArgumentCompleter : IArgumentCompleter
{
    /// <summary>
    /// The drivers docker ships with, so that -Driver still completes on a daemon that
    /// has no network using them yet.
    /// </summary>
    private static readonly string[] BuiltInDrivers = ["bridge", "host", "ipvlan", "macvlan", "none", "overlay"];

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

        var networks = client.Networks.ListNetworksAsync().GetAwaiter().GetResult();

        // Cmdlets such as Get-ContainerNet keep Id and Name in separate parameter sets, so
        // complete each one with just the matching field.
        IEnumerable<string> candidates = parameterName switch
        {
            "Driver" => networks.Select(network => network.Driver).Concat(BuiltInDrivers),
            "Scope" => networks.Select(network => network.Scope),
            "Id" => networks.Select(network => network.ID),
            "Name" => networks.Select(network => network.Name),
            // If the user has already typed part of the name, then include IDs that start
            // with that portion. Otherwise, just let the user tab through the names.
            _ => wordToComplete.Length == 0
                ? networks.Select(network => network.Name)
                : networks.SelectMany(network => new[] { network.Name, network.ID }),
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
