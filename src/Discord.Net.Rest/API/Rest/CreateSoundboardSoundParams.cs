using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class CreateSoundboardSoundParams
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public Optional<string> EmojiName { get; set; }

    [JsonProperty("volume")]
    public double Volume { get; set; }

    [JsonProperty("sound")]
    public Sound Sound { get; set; }
}
