using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data relating to an invite update.
/// </summary>
public class SocketInviteUpdateAuditLogData : ISocketAuditLogData
{
    private SocketInviteUpdateAuditLogData(SocketInviteInfo before, SocketInviteInfo after)
    {
        Before = before;
        After = after;
    }

    internal static SocketInviteUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<InviteInfoAuditLogModel>(changes, discord);

        return new SocketInviteUpdateAuditLogData(new(before), new(after));
    }

    /// <summary>
    ///     Gets the invite information before the changes.
    /// </summary>
    /// <returns>
    ///     An information object containing the original invite information before the changes were made.
    /// </returns>
    public SocketInviteInfo Before { get; }

    /// <summary>
    ///     Gets the invite information after the changes.
    /// </summary>
    /// <returns>
    ///     An information object containing the invite information after the changes were made.
    /// </returns>
    public SocketInviteInfo After { get; }
}
