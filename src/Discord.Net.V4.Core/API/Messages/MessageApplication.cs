using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class MessageApplication
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("cover_image")]
    public required string CoverImage { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
