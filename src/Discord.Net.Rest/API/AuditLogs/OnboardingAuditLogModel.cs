using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class OnboardingAuditLogModel : IAuditLogInfoModel
{
    [JsonField("default_channel_ids")]
    public ulong[] DefaultChannelIds { get; set; }

    [JsonField("prompts")]
    public GuildOnboardingPrompt[] Prompts { get; set; }

    [JsonField("enabled")]
    public bool? Enabled { get; set; }
}
