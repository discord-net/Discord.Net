using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class EmbedImage : IEmbedImageModel
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public Optional<string> ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public Optional<int> Height { get; set; }

    [JsonPropertyName("width")]
    public Optional<int> Width { get; set; }

    string? IEmbedImageModel.ProxyUrl => ~ProxyUrl;
    int? IEmbedImageModel.Height => ~Height;
    int? IEmbedImageModel.Width => ~Width;
}
