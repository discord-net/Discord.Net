using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class EmbedField : IEmbedFieldModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }

    [JsonPropertyName("inline")]
    public Optional<bool> Inline { get; set; }

    bool? IEmbedFieldModel.Inline => ~Inline;
}
