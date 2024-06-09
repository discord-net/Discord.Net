using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class StickerItem
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("format_type")]
    public int FormatType { get; set; }
}
