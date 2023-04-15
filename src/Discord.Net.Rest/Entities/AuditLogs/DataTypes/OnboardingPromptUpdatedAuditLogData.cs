using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;


/// <summary>
///     Contains a piece of audit log data related to an onboarding prompt update.
/// </summary>
public class OnboardingPromptUpdatedAuditLogData : IAuditLogData
{
    internal OnboardingPromptUpdatedAuditLogData(OnboardingPromptInfo before, OnboardingPromptInfo after)
    {
        Before = before;
        After = after;
    }

    internal static OnboardingPromptUpdatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<OnboardingPromptAuditLogModel>(changes, discord);

        return new OnboardingPromptUpdatedAuditLogData(new(before, discord), new(after, discord));
    }

    /// <summary>
    ///     Gets the onboarding prompt information after the changes.
    /// </summary>
    OnboardingPromptInfo After { get; set; }

    /// <summary>
    ///     Gets the onboarding prompt information before the changes.
    /// </summary>
    OnboardingPromptInfo Before { get; set; }
}
