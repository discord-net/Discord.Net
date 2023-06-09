using Newtonsoft.Json;

namespace Discord.API;

internal class SoundboardSound
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("volume")]
    public double Volume { get; set; }

    [JsonProperty("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public Optional<string> EmojiName { get; set; }

    [JsonProperty("override_path")]
    public string OverridePath { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("user")]
    public Optional<User> User { get; set; }

    [JsonProperty("available")]
    public Optional<bool> Available { get; set; }
}
