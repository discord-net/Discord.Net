using Newtonsoft.Json;

namespace Discord.API;

internal class GuildOnboardingPrompt
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("options")]
    public GuildOnboardingPromptOption[] Options { get; set; }

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
