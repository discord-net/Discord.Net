using System.Text.Json.Serialization;

namespace Discord.Models;

public sealed class ModifyGuildSoundboardSoundParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }
    
    [JsonPropertyName("volume")]
    public Optional<double?> Volume { get; set; }
    
    [JsonPropertyName("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }
    
    [JsonPropertyName("emoji_name")]
    public Optional<string?> EmojiName { get; set; }
}