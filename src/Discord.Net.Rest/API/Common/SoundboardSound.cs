using Newtonsoft.Json;

namespace Discord.API;

internal class SoundboardSound
{
    [JsonProperty("sound_id")]
    public ulong SoundId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("volume")]
    public double Volume { get; set; }

    [JsonProperty("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public Optional<string> EmojiName { get; set; }

    [JsonProperty("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("user")]
    public Optional<User> User { get; set; }

    [JsonProperty("available")]
    public Optional<bool> IsAvailable { get; set; }
}
