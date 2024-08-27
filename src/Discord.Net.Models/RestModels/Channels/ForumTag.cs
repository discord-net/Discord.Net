using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ForumTag : ITagModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("moderated")]
    public bool Moderated { get; set; }
    
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }

    DiscordEmojiId? ITagModel.Emoji 
        => EmojiName is not null || EmojiId.HasValue ? new(EmojiName, EmojiId, false) : null;

}
