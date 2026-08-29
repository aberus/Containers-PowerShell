using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

/// <summary>
/// Completes image names and ids from the session's image list. Only a session already
/// running in this process is asked, so tab completion never starts one.
/// </summary>
public class ImageArgumentCompleter : IArgumentCompleter
{
    private const string LatestSuffix = ":latest";

    /// <summary>
    /// Returns the completions that start with the text typed so far.
    /// </summary>
    public IEnumerable<CompletionResult> CompleteArgument(string commandName,
                                                          string parameterName,
                                                          string wordToComplete,
                                                          CommandAst commandAst,
                                                          IDictionary fakeBoundParameters)
    {
        var sessionName = fakeBoundParameters?["SessionName"] as string ?? WslcRuntime.DefaultSessionName;
        if (!WslcRuntime.TryGetSession(sessionName, out var session) || session is null)
        {
            return [];
        }

        return session.GetImages()
            .SelectMany(image => wordToComplete.Length == 0
                ? [image.Name]
                // Pushing takes a name, not an id, so ids are only offered elsewhere.
                : new[] { image.Name, ParameterResolvers.GetImageId(image) ?? image.Name })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(name => name.StartsWith(wordToComplete, StringComparison.CurrentCultureIgnoreCase))
            .Select(name => name.EndsWith(LatestSuffix) && wordToComplete.Length <= name.Length - LatestSuffix.Length
                // Hide ":latest" unless the user has started typing it.
                ? name[..^LatestSuffix.Length]
                : name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .Select(name => new CompletionResult(name, name, CompletionResultType.Text, name));
    }
}
