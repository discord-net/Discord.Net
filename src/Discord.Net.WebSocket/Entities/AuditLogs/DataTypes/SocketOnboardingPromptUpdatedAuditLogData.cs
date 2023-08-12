using Discord.Rest;
using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;


/// <summary>
///     Contains a piece of audit log data related to an onboarding prompt update.
/// </summary>
public class SocketOnboardingPromptUpdatedAuditLogData : ISocketAuditLogData
{
    internal SocketOnboardingPromptUpdatedAuditLogData(SocketOnboardingPromptInfo before, SocketOnboardingPromptInfo after)
    {
        Before = before;
        After = after;
    }

    internal static SocketOnboardingPromptUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<OnboardingPromptAuditLogModel>(changes, discord);

        return new SocketOnboardingPromptUpdatedAuditLogData(new(before, discord), new(after, discord));
    }

    /// <summary>
    ///     Gets the onboarding prompt information after the changes.
    /// </summary>
    SocketOnboardingPromptInfo After { get; set; }

    /// <summary>
    ///     Gets the onboarding prompt information before the changes.
    /// </summary>
    SocketOnboardingPromptInfo Before { get; set; }
}
