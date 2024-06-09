using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class SelectMenuOption : IEntityModelSource
{
    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<IEmote> Emoji { get; set; }

    [JsonPropertyName("default")]
    public Optional<bool> IsDefault { get; set; }

    public IEnumerable<IEntityModel> GetEntities()
    {
        if (Emoji.IsSpecified)
            yield return Emoji.Value;
    }
}