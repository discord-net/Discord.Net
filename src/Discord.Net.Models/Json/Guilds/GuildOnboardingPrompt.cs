using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildOnboardingPrompt
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("options")]
    public required GuildOnboardingPromptOption[] Options { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("single_select")]
    public bool IsSingleSelect { get; set; }

    [JsonPropertyName("required")]
    public bool IsRequired { get; set; }

    [JsonPropertyName("in_onboarding")]
    public bool IsInOnboarding { get; set; }
}
