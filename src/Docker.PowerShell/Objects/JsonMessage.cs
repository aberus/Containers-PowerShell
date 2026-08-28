using System.Text.Json.Serialization;

namespace Docker.PowerShell.Objects;

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

internal class JsonProgress
{
    [JsonPropertyName("current")]
    public long Current { get; set; }

    [JsonPropertyName("total")]
    public long Total { get; set; }

    [JsonPropertyName("start")]
    public long Start { get; set; }
}

internal class JsonError
{
    [JsonPropertyName("code")]
    public long Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}