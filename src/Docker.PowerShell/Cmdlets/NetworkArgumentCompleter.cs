using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Linq;
using System;

namespace Docker.PowerShell.Cmdlets;

public class NetworkArgumentCompleter : IArgumentCompleter
{
    public IEnumerable<CompletionResult> CompleteArgument(string commandName,
                                                          string parameterName,
                                                          string wordToComplete,
                                                          CommandAst commandAst,
                                                          IDictionary fakeBoundParameters)
    {
        var client = DockerFactory.CreateClient(fakeBoundParameters);

        var result = client.Networks.ListNetworksAsync().GetAwaiter().GetResult();

        return result.SelectMany(network =>
            {
                // If the user has already typed part of the name, then include IDs that start
                // with that portion. Otherwise, just let the user tab through the names.

                // Special handling for Get-networkNetwork, where Id an Name are separate parameters.
                if (commandName == "Get-ContainerNet" && parameterName == "Id")
                {
                    return [network.ID];
                }
                else if (wordToComplete?.Length == 0 || parameterName == "Name")
                {
                    return [network.Name];
                }
                else if (wordToComplete?.Length == 0 || parameterName == "Driver")
                {
                    return [network.Driver];
                }
                else if (wordToComplete?.Length == 0 || parameterName == "Scope")
                {
                    return [network.Scope];
                }
                else
                {
                    return (List<string>)[network.Name, network.ID];
                }
            })
            .Distinct()
            .Where(name => name.StartsWith(wordToComplete, StringComparison.CurrentCultureIgnoreCase))
            .OrderBy(name => name)
            .Select(name => name.Contains(" ") ? "\"" + name + "\"" : name)
            .Select(name => new CompletionResult(name, name, CompletionResultType.Text, name));
    }
}