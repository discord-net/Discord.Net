using Newtonsoft.Json;

namespace Discord.API;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal class ModifyForumReactionEmojiParams
{
    [JsonProperty("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public Optional<string> EmojiName { get; set; }
}


