using Newtonsoft.Json;

namespace Discord.API;

internal class GuildOnboardingPromptOption
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("channel_ids")]
    public ulong[] ChannelIds { get; set; }

    [JsonProperty("role_ids")]
    public ulong[] RoleIds { get; set; }

    [JsonProperty("emoji")]
    public Emoji Emoji { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}
