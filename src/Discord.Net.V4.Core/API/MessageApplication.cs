using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class MessageApplication
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("cover_image")]
    public string CoverImage { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
