using System.Text.Json.Serialization;

namespace Discord.API;

internal class DefaultReaction
{
    [JsonPropertyName("emoji_id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("emoji_name")]
    public string Name { get; set; }
}
