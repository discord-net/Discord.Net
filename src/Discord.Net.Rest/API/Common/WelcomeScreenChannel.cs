using Newtonsoft.Json;

namespace Discord.API;

internal class WelcomeScreenChannel
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public Optional<string> EmojiName { get; set; }
}
