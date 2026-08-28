using System.Text.Json.Serialization;

namespace Docker.PowerShell.Objects;

/// <summary>
/// One message from a daemon progress stream, as sent while building, pulling, or pushing.
/// </summary>
internal class JsonMessage
{
    [JsonPropertyName("stream")]
    public string Stream { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("progressDetail")]
    public JsonProgress Progress { get; set; }

    [JsonPropertyName("id")]
    public string ID { get; set; }

    [JsonPropertyName("from")]
    public string From { get; set; }

    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("timeNano")]
    public long TimeNano { get; set; }

    [JsonPropertyName("errorDetail")]
    public JsonError Error { get; set; }
}

/// <summary>
/// How far a single layer's transfer has got.
/// </summary>
internal class JsonProgress
{
    [JsonPropertyName("current")]
    public long Current { get; set; }

    [JsonPropertyName("total")]
    public long Total { get; set; }

    [JsonPropertyName("start")]
    public long Start { get; set; }
}

/// <summary>
/// An error reported partway through a progress stream.
/// </summary>
internal class JsonError
{
    [JsonPropertyName("code")]
    public long Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}