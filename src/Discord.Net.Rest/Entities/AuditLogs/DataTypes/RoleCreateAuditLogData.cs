using Discord.API.AuditLogs;

using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a role creation.
/// </summary>
public class RoleCreateAuditLogData : IAuditLogData
{
    private RoleCreateAuditLogData(ulong id, RoleEditInfo props)
    {
        RoleId = id;
        Properties = props;
    }

    internal static RoleCreateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<RoleInfoAuditLogModel>(changes, discord);

        return new RoleCreateAuditLogData(entry.TargetId!.Value,
            new RoleEditInfo(data));
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
    public RoleEditInfo Properties { get; }
}
