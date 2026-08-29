using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

/// <summary>
/// Completes volume names from the ones this module has created. The SDK cannot enumerate
/// volumes, so a volume made by the wslc CLI is not offered.
/// </summary>
public class VolumeArgumentCompleter : IArgumentCompleter
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

        return VolumeIndex.Read(sessionName)
            .Where(name => name.StartsWith(wordToComplete, StringComparison.CurrentCultureIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .Select(name => new CompletionResult(name, name, CompletionResultType.Text, name));
    }
}
