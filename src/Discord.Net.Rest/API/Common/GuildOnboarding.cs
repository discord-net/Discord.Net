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

    // TODO: Undocumented props 
    [JsonProperty("enable_onboarding_prompts")]
    public bool EnableOnboardingPrompts { get; set; }

    [JsonProperty("enable_default_channels")]
    public bool EnableDefaultChannels { get; set; }
}
