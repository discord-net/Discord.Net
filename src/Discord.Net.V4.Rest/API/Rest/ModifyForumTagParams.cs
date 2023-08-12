using System.Text.Json.Serialization;

namespace Discord.API;

internal class ModifyForumTagParams
{
    [JsonPropertyName("id")]
    public Optional<ulong> Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }

    [JsonPropertyName("emoji_name")]
    public Optional<string> EmojiName { get; set; }

    [JsonPropertyName("moderated")]
    public bool Moderated { get; set; }
}
