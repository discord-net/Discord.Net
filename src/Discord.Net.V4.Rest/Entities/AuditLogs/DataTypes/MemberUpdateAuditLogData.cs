using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a change in a guild member.
/// </summary>
public class MemberUpdateAuditLogData : IAuditLogData
{
    private MemberUpdateAuditLogData(IUser target, MemberInfo before, MemberInfo after)
    {
        Target = target;
        Before = before;
        After = after;
    }

    internal static MemberUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<MemberInfoAuditLogModel>(changes, discord);

        var targetInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
        RestUser user = (targetInfo != null) ? RestUser.Create(discord, targetInfo) : null;

        return new MemberUpdateAuditLogData(user, new MemberInfo(before), new MemberInfo(after));
    }

    /// <summary>
    ///     Gets the user that the changes were performed on.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A user object representing the user who the changes were performed on.
    /// </returns>
    public IUser Target { get; }
    /// <summary>
    ///     Gets the member information before the changes.
    /// </summary>
    /// <returns>
    ///     An information object containing the original member information before the changes were made.
    /// </returns>
    public MemberInfo Before { get; }
    /// <summary>
    ///     Gets the member information after the changes.
    /// </summary>
    /// <returns>
    ///     An information object containing the member information after the changes were made.
    /// </returns>
    public MemberInfo After { get; }
}
