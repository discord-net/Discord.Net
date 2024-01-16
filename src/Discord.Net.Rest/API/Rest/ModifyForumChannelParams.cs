using Newtonsoft.Json;

namespace Discord.API.Rest;


[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal class ModifyForumChannelParams : ModifyTextChannelParams
{
    [JsonProperty("available_tags")]
    public Optional<ModifyForumTagParams[]> Tags { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public Optional<int> ThreadCreationInterval { get; set; }

    [JsonProperty("default_reaction_emoji")]
    public Optional<ModifyForumReactionEmojiParams> DefaultReactionEmoji { get; set; }

    [JsonProperty("default_sort_order")]
    public Optional<ForumSortOrder> DefaultSortOrder { get; set; }

    [JsonProperty("default_forum_layout")]
    public Optional<ForumLayout> DefaultLayout { get; set; }
}
