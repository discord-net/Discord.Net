using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an onboarding prompt creation.
/// </summary>
public class SocketOnboardingPromptCreatedAuditLogData : ISocketAuditLogData
{
    internal SocketOnboardingPromptCreatedAuditLogData(SocketOnboardingPromptInfo data)
    {
        Data = data;
    }

    internal static SocketOnboardingPromptCreatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<OnboardingPromptAuditLogModel>(changes, discord);

        return new SocketOnboardingPromptCreatedAuditLogData(new(data, discord));
    }

    /// <summary>
    ///     Gets the onboarding prompt information after the changes.
    /// </summary>
    SocketOnboardingPromptInfo Data { get; set; }
}
