using Newtonsoft.Json;

namespace Discord.API;

internal class ForumReactionEmoji
{
    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public Optional<string> EmojiName { get; set; }
}
