using Newtonsoft.Json;

namespace Discord.API.Rest;


[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal class ModifyForumChannelParams : ModifyTextChannelParams
{
    [JsonProperty("available_tags")]
    public Optional<ModifyForumTagParams[]> Tags { get; set; }

    [JsonProperty("default_thread_rate_limit_per_user")]
    public Optional<int> DefaultSlowModeInterval { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public Optional<int> ThreadCreationInterval { get; set; }

    [JsonProperty("default_reaction_emoji")]
    public Optional<ModifyForumReactionEmojiParams> DefaultReactionEmoji { get; set; }

    [JsonProperty("default_sort_order")]
    public Optional<ForumSortOrder> DefaultSortOrder { get; set; }
}
