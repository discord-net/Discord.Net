using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data relating to a role deletion.
/// </summary>
public class RoleDeleteAuditLogData : IAuditLogData
{
    private RoleDeleteAuditLogData(ulong id, RoleEditInfo props)
    {
        RoleId = id;
        Properties = props;
    }

    internal static RoleDeleteAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<RoleInfoAuditLogModel>(changes, discord);

        return new RoleDeleteAuditLogData(entry.TargetId!.Value, new RoleEditInfo(data));
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
    public RoleEditInfo Properties { get; }
}
