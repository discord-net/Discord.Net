using System.Text.Json.Serialization;

namespace Discord.Models;

public sealed class CreateGuildSoundboardSoundParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("sound")]
    public required string Sound { get; set; }
    
    [JsonPropertyName("volume")]
    public Optional<double?> Volume { get; set; }
    
    [JsonPropertyName("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }
    
    [JsonPropertyName("emoji_name")]
    public Optional<string?> EmojiName { get; set; }
}