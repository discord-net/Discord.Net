using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class EmbedFooter
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("icon_url")]
    public Optional<string> IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public Optional<string> ProxyIconUrl { get; set; }
}
