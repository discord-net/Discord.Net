using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class EmbedThumbnail : IEmbedThumbnailModel
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public Optional<string> ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public Optional<int> Height { get; set; }

    [JsonPropertyName("width")]
    public Optional<int> Width { get; set; }

    string? IEmbedThumbnailModel.ProxyUrl => ~ProxyUrl;
    int? IEmbedThumbnailModel.Height => ~Height;
    int? IEmbedThumbnailModel.Width => ~Width;
}
