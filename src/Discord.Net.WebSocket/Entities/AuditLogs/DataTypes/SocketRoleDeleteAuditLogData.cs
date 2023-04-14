using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLogs.RoleInfoAuditLogModel;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data relating to a role deletion.
/// </summary>
public class SocketRoleDeleteAuditLogData : ISocketAuditLogData
{
    private SocketRoleDeleteAuditLogData(ulong id, SocketRoleEditInfo props)
    {
        RoleId = id;
        Properties = props;
    }

    internal static SocketRoleDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<Model>(changes, discord);

        return new SocketRoleDeleteAuditLogData(entry.TargetId!.Value,
            new SocketRoleEditInfo(data));
    }

    /// <summary>
    ///     Gets the ID of the role that was deleted.
    /// </summary>
    /// <return>
    ///     A <see cref="ulong"/> representing the snowflake identifier to the role that was deleted.
    /// </return>
    public ulong RoleId { get; }

    /// <summary>
    ///     Gets the role information that was deleted.
    /// </summary>
    /// <return>
    ///     An information object representing the properties of the role that was deleted.
    /// </return>
    public SocketRoleEditInfo Properties { get; }
}
