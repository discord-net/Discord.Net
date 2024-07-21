using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class EmbedProvider : IEmbedProviderModel
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    string? IEmbedProviderModel.Url => ~Url;
    string? IEmbedProviderModel.Name => ~Name;
}
