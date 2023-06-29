using System.Text.Json.Serialization;

namespace Discord.API;

internal class EmbedField
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("inline")]
    public Optional<bool> Inline { get; set; }
}
