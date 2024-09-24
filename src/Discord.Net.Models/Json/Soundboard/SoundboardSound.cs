using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType]
public class SoundboardSound : ISoundboardSoundModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("sound_id")]
    public ulong Id { get; set; }
    
    [JsonPropertyName("volume")]
    public double Volume { get; set; }
    
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }
    
    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }
    
    [JsonPropertyName("available")]
    public bool IsAvailable { get; set; }
}