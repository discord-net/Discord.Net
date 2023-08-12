using System.Text.Json.Serialization;

namespace Discord.API;

internal class StickerItem
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("format_type")]
    public StickerFormatType FormatType { get; set; }
}
