namespace Discord.WebSocket;

using Discord.Rest;
using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;

/// <summary>
///     Contains a piece of audit log data related to a guild update.
/// </summary>
public class SocketOnboardingUpdatedAuditLogData : ISocketAuditLogData
{
    internal SocketOnboardingUpdatedAuditLogData(SocketOnboardingInfo before, SocketOnboardingInfo after)
    {
        Before = before;
        After = after;
    }

    internal static SocketOnboardingUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<OnboardingAuditLogModel>(changes, discord);

        return new SocketOnboardingUpdatedAuditLogData(new(before, discord), new(after, discord));
    }

    /// <summary>
    ///     Gets the onboarding information after the changes.
    /// </summary>
    SocketOnboardingInfo After { get; set; }

    /// <summary>
    ///     Gets the onboarding information before the changes.
    /// </summary>
    SocketOnboardingInfo Before { get; set; }
}
