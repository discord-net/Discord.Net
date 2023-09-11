using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class EmbedVideo
{
    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public Optional<string> ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public Optional<int> Height { get; set; }

    [JsonPropertyName("width")]
    public Optional<int> Width { get; set; }
}
