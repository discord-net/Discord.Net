using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class EmbedAuthor : IEmbedAuthorModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    [JsonPropertyName("icon_url")]
    public Optional<string> IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public Optional<string> ProxyIconUrl { get; set; }

    string? IEmbedAuthorModel.Url => ~Url;
    string? IEmbedAuthorModel.IconUrl => ~IconUrl;
    string? IEmbedAuthorModel.ProxyIconUrl => ~ProxyIconUrl;
}
