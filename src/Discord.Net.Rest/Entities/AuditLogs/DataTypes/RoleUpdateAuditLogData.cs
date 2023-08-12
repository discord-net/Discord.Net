using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a role update.
/// </summary>
public class RoleUpdateAuditLogData : IAuditLogData
{
    private RoleUpdateAuditLogData(ulong id, RoleEditInfo oldProps, RoleEditInfo newProps)
    {
        RoleId = id;
        Before = oldProps;
        After = newProps;
    }

    internal static RoleUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<RoleInfoAuditLogModel>(changes, discord);

        return new RoleUpdateAuditLogData(entry.TargetId!.Value, new(before), new(after));
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
    public RoleEditInfo Before { get; }

    /// <summary>
    ///     Gets the role information after the changes.
    /// </summary>
    /// <returns>
    ///     A role information object containing the role information after the changes were made.
    /// </returns>
    public RoleEditInfo After { get; }
}
