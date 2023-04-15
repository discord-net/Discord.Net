using Discord.API.AuditLogs;

using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data relating to an invite update.
/// </summary>
public class InviteUpdateAuditLogData : IAuditLogData
{
    private InviteUpdateAuditLogData(InviteInfo before, InviteInfo after)
    {
        Before = before;
        After = after;
    }

    internal static InviteUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<InviteInfoAuditLogModel>(changes, discord);

        return new InviteUpdateAuditLogData(new(before), new(after));
    }

    /// <summary>
    ///     Gets the invite information before the changes.
    /// </summary>
    /// <returns>
    ///     An information object containing the original invite information before the changes were made.
    /// </returns>
    public InviteInfo Before { get; }

    /// <summary>
    ///     Gets the invite information after the changes.
    /// </summary>
    /// <returns>
    ///     An information object containing the invite information after the changes were made.
    /// </returns>
    public InviteInfo After { get; }
}
