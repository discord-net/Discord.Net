using Discord.Rest;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLogs.RoleInfoAuditLogModel;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a role creation.
/// </summary>
public class SocketRoleCreateAuditLogData : ISocketAuditLogData
{
    private SocketRoleCreateAuditLogData(ulong id, SocketRoleEditInfo props)
    {
        RoleId = id;
        Properties = props;
    }

    internal static SocketRoleCreateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<Model>(changes, discord);

        return new SocketRoleCreateAuditLogData(entry.TargetId!.Value,
            new SocketRoleEditInfo(data));
    }

    /// <summary>
    ///     Gets the ID of the role that was created.
    /// </summary>
    /// <return>
    ///     A <see cref="ulong"/> representing the snowflake identifier to the role that was created.
    /// </return>
    public ulong RoleId { get; }

    /// <summary>
    ///     Gets the role information that was created.
    /// </summary>
    /// <return>
    ///     An information object representing the properties of the role that was created.
    /// </return>
    public SocketRoleEditInfo Properties { get; }
}
