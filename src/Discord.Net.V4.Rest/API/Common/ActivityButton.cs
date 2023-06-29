using System.Text.Json.Serialization;

namespace Discord.API;

internal class ActivityButton
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
