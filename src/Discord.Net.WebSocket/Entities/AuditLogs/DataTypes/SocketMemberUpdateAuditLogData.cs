using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a change in a guild member.
/// </summary>
public class SocketMemberUpdateAuditLogData : ISocketAuditLogData
{
    private SocketMemberUpdateAuditLogData(Cacheable<SocketUser, RestUser, IUser, ulong> target, MemberInfo before, MemberInfo after)
    {
        Target = target;
        Before = before;
        After = after;
    }

    internal static SocketMemberUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<MemberInfoAuditLogModel>(changes, discord);

        var cachedUser = discord.GetUser(entry.TargetId!.Value);
        var cacheableUser = new Cacheable<SocketUser, RestUser, IUser, ulong>(
            cachedUser,
            entry.TargetId.Value,
            cachedUser is not null,
            async () =>
            {
                var user = await discord.ApiClient.GetUserAsync(entry.TargetId.Value);
                return user is not null ? RestUser.Create(discord, user) : null;
            });

        return new SocketMemberUpdateAuditLogData(cacheableUser, new MemberInfo(before), new MemberInfo(after));
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
    public Cacheable<SocketUser, RestUser, IUser, ulong> Target { get; }

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
