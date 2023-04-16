using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class OnboardingPromptAuditLogModel : IAuditLogInfoModel
{
    [JsonField("id")]
    public ulong? Id { get; set; }

    [JsonField("title")]
    public string Title { get; set; }

    [JsonField("options")]
    public GuildOnboardingPromptOption[] Options { get; set; }

    [JsonField("single_select")]
    public bool? IsSingleSelect { get; set; }

    [JsonField("required")]
    public bool? IsRequired { get; set; }

    [JsonField("in_onboarding")]
    public bool? IsInOnboarding { get; set; }

    [JsonField("type")]
    public GuildOnboardingPromptType? Type { get; set; }
}
