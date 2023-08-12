using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class ModifyGuildOnboardingParams
{
    [JsonPropertyName("prompts")]
    public Optional<GuildOnboardingPromptParams[]> Prompts { get; set; }

    [JsonPropertyName("default_channel_ids")]
    public Optional<ulong[]> DefaultChannelIds { get; set; }

    [JsonPropertyName("enabled")]
    public Optional<bool> Enabled { get; set; }

    [JsonPropertyName("mode")]
    public Optional<GuildOnboardingMode> Mode { get; set; }
}


internal class GuildOnboardingPromptParams
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("options")]
    public GuildOnboardingPromptOptionParams[] Options { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("single_select")]
    public bool IsSingleSelect { get; set; }

    [JsonPropertyName("required")]
    public bool IsRequired { get; set; }

    [JsonPropertyName("in_onboarding")]
    public bool IsInOnboarding { get; set; }

    [JsonPropertyName("type")]
    public GuildOnboardingPromptType Type { get; set; }
}


internal class GuildOnboardingPromptOptionParams
{
    [JsonPropertyName("id")]
    public Optional<ulong> Id { get; set; }

    [JsonPropertyName("channel_ids")]
    public ulong[] ChannelIds { get; set; }

    [JsonPropertyName("role_ids")]
    public ulong[] RoleIds { get; set; }

    [JsonPropertyName("emoji_name")]
    public string EmojiName { get; set; }

    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonPropertyName("emoji_animated")]
    public bool? EmojiAnimated { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}
