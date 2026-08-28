using System.Linq;
using System.Management.Automation;
using System.Text.Json;

namespace WSLC.PowerShell.Support;

internal static class JsonConversion
{
    /// <summary>
    /// Converts a JSON document (e.g. from Container.Inspect) into PSObjects so the
    /// result is navigable in PowerShell.
    /// </summary>
    public static object? ToPSObject(string json)
    {
        using var document = JsonDocument.Parse(json);
        return ToPSObject(document.RootElement);
    }

    private static object? ToPSObject(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var result = new PSObject();
                foreach (var property in element.EnumerateObject())
                {
                    result.Properties.Add(new PSNoteProperty(property.Name, ToPSObject(property.Value)));
                }

                return result;
            case JsonValueKind.Array:
                return element.EnumerateArray().Select(ToPSObject).ToArray();
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                return element.TryGetInt64(out var integer) ? integer : element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            default:
                return null;
        }
    }
}
