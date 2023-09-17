using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class StickerItem
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("format_type")]
    public StickerFormatType FormatType { get; set; }
}