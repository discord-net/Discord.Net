using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;


/// <summary>
///     Contains a piece of audit log data related to a guild update.
/// </summary>
public class OnboardingUpdatedAuditLogData : IAuditLogData
{
    internal OnboardingUpdatedAuditLogData(OnboardingInfo before, OnboardingInfo after)
    {
        Before = before;
        After = after;
    }

    internal static OnboardingUpdatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<OnboardingAuditLogModel>(changes, discord);

        return new OnboardingUpdatedAuditLogData(new(before, discord), new(after, discord));
    }

    /// <summary>
    ///     Gets the onboarding information after the changes.
    /// </summary>
    OnboardingInfo After { get; set; }

    /// <summary>
    ///     Gets the onboarding information before the changes.
    /// </summary>
    OnboardingInfo Before { get; set; }
}
