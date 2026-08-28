using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Support;

/// <summary>
/// Helpers invoked from WSLC.Format.ps1xml. Public because format-file script blocks
/// call them by full type name.
/// </summary>
public static class Formatter
{
    private const int ShortIdLength = 12;

    public static string TruncateId(string id)
    {
        var index = id.IndexOf(':');
        if (index >= 0)
        {
            id = id.Substring(index + 1);
        }

        if (id.Length > ShortIdLength)
        {
            id = id.Substring(0, ShortIdLength);
        }

        return id;
    }

    public static string ContainerName(Container container)
    {
        var name = WslcRuntime.GetContainerName(container);
        return name == container.Id ? TruncateId(name) : name;
    }

    public static string ContainerSessionName(Container container)
    {
        return WslcRuntime.GetContainerSessionName(WslcRuntime.GetContainerName(container)) ?? string.Empty;
    }

    public static string ImageId(ImageInfo image)
    {
        var id = ParameterResolvers.GetImageId(image);
        return id is null ? string.Empty : TruncateId(id);
    }

    public static string SessionName(Session session)
    {
        return WslcRuntime.TryGetSessionName(session, out var name) ? name : string.Empty;
    }

    public static int SessionContainerCount(Session session)
    {
        return WslcRuntime.TryGetSessionName(session, out var name)
            ? WslcRuntime.GetContainers(name).Length
            : 0;
    }
}
