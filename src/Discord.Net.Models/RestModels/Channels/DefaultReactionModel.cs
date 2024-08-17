using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class DefaultReactionModel
{
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }
    
    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }
}