using Discord.Rest;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLogs.RoleInfoAuditLogModel;


namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a role update.
/// </summary>
public class SocketRoleUpdateAuditLogData : ISocketAuditLogData
{
    private SocketRoleUpdateAuditLogData(ulong id, SocketRoleEditInfo oldProps, SocketRoleEditInfo newProps)
    {
        RoleId = id;
        Before = oldProps;
        After = newProps;
    }

    internal static SocketRoleUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<Model>(changes, discord);

        return new SocketRoleUpdateAuditLogData(entry.TargetId!.Value, new(before), new(after));
    }

    /// <summary>
    ///     Gets the ID of the role that was changed.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the role that was changed.
    /// </returns>
    public ulong RoleId { get; }
    /// <summary>
    ///     Gets the role information before the changes.
    /// </summary>
    /// <returns>
    ///     A role information object containing the role information before the changes were made.
    /// </returns>
    public SocketRoleEditInfo Before { get; }
    /// <summary>
    ///     Gets the role information after the changes.
    /// </summary>
    /// <returns>
    ///     A role information object containing the role information after the changes were made.
    /// </returns>
    public SocketRoleEditInfo After { get; }
}
