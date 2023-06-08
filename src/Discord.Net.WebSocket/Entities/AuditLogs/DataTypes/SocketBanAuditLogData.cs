using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a ban.
/// </summary>
public class SocketBanAuditLogData : ISocketAuditLogData
{
    private SocketBanAuditLogData(Cacheable<SocketUser, RestUser, IUser, ulong> user)
    {
        Target = user;
    }

    internal static SocketBanAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
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

        return new SocketBanAuditLogData(cacheableUser);
    }

    /// <summary>
    ///     Gets the user that was banned.
    /// </summary>
    /// <remarks>
    ///     Download method may return <see langword="null"/> if the user is a 'Deleted User#....'
    ///     because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A cacheable user object representing the banned user.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong> Target { get; }
}
