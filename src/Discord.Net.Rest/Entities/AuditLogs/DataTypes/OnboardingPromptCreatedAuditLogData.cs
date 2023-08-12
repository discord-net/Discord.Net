using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an onboarding prompt creation.
/// </summary>
public class OnboardingPromptCreatedAuditLogData : IAuditLogData
{
    internal OnboardingPromptCreatedAuditLogData(OnboardingPromptInfo data)
    {
        Data = data;
    }

    internal static OnboardingPromptCreatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<OnboardingPromptAuditLogModel>(changes, discord);

        return new OnboardingPromptCreatedAuditLogData(new(data, discord));
    }

    /// <summary>
    ///     Gets the onboarding prompt information after the changes.
    /// </summary>
    OnboardingPromptInfo Data { get; set; }
}
