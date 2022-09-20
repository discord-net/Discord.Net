using Newtonsoft.Json;

namespace Discord.API.Rest;


[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal class ModifyForumChannelParams : ModifyTextChannelParams
{
    [JsonProperty("available_tags")]
    public Optional<ForumTagParams[]> Tags { get; set; }

    [JsonProperty("default_thread_rate_limit_per_user")]
    public Optional<int> DefaultSlowModeInterval { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public Optional<int> ThreadCreationInterval { get; set; }
}
