using Newtonsoft.Json;

namespace Discord.API;

internal class GuildOnboarding
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("prompts")]
    public GuildOnboardingPrompt[] Prompts { get; set; }

    [JsonProperty("default_channel_ids")]
    public ulong[] DefaultChannelIds { get; set; }

    [JsonProperty("enabled")]
    public bool Enabled { get; set; }
}
