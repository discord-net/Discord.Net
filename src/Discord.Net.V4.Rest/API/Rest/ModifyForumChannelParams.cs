using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyForumChannelParams : ModifyTextChannelParams
{
    [JsonPropertyName("available_tags")]
    public Optional<ModifyForumTagParams[]> Tags { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int> ThreadCreationInterval { get; set; }

    [JsonPropertyName("default_reaction_emoji")]
    public Optional<ModifyForumReactionEmojiParams> DefaultReactionEmoji { get; set; }

    [JsonPropertyName("default_sort_order")]
    public Optional<ForumSortOrder> DefaultSortOrder { get; set; }

    [JsonPropertyName("default_forum_layout")]
    public Optional<ForumLayout> DefaultLayout { get; set; }
}
