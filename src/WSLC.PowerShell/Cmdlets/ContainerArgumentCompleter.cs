using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

/// <summary>
/// Completes container names and ids from the module's own record, which also covers
/// containers created by an earlier PowerShell process. Completion never starts a session:
/// the record is read from disk, so tab completion cannot boot a VM.
/// </summary>
public class ContainerArgumentCompleter : IArgumentCompleter
{
    /// <summary>
    /// Returns the completions that start with the text typed so far.
    /// </summary>
    public IEnumerable<CompletionResult> CompleteArgument(string commandName,
                                                          string parameterName,
                                                          string wordToComplete,
                                                          CommandAst commandAst,
                                                          IDictionary fakeBoundParameters)
    {
        var sessionName = fakeBoundParameters?["SessionName"] as string;

        var names = WslcRuntime.GetContainerCandidates(sessionName)
            .SelectMany(candidate => wordToComplete.Length == 0
                ? [candidate.Name]
                // Once part of a name is typed, ids that start with it are worth offering too.
                : new[] { candidate.Name, Formatter.TruncateId(candidate.Id) })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(name => name.StartsWith(wordToComplete, StringComparison.CurrentCultureIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

        return names.Select(name => new CompletionResult(name, name, CompletionResultType.Text, name));
    }
}
