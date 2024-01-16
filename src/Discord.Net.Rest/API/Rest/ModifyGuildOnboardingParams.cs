using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class ModifyGuildOnboardingParams
{
    [JsonProperty("prompts")]
    public Optional<GuildOnboardingPromptParams[]> Prompts { get; set; }

    [JsonProperty("default_channel_ids")]
    public Optional<ulong[]> DefaultChannelIds { get; set; }

    [JsonProperty("enabled")]
    public Optional<bool> Enabled { get; set; }

    [JsonProperty("mode")]
    public Optional<GuildOnboardingMode> Mode { get; set; }
}


internal class GuildOnboardingPromptParams
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("options")]
    public GuildOnboardingPromptOptionParams[] Options { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("single_select")]
    public bool IsSingleSelect { get; set; }

    [JsonProperty("required")]
    public bool IsRequired { get; set; }

    [JsonProperty("in_onboarding")]
    public bool IsInOnboarding { get; set; }

    [JsonProperty("type")]
    public GuildOnboardingPromptType Type { get; set; }
}


internal class GuildOnboardingPromptOptionParams
{
    [JsonProperty("id")]
    public Optional<ulong> Id { get; set; }

    [JsonProperty("channel_ids")]
    public ulong[] ChannelIds { get; set; }

    [JsonProperty("role_ids")]
    public ulong[] RoleIds { get; set; }

    [JsonProperty("emoji_name")]
    public string EmojiName { get; set; }

    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonProperty("emoji_animated")]
    public bool? EmojiAnimated { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}
