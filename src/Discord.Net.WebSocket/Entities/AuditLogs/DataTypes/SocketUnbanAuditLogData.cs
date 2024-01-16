using Discord.Rest;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an unban.
/// </summary>
public class SocketUnbanAuditLogData : ISocketAuditLogData
{
    private SocketUnbanAuditLogData(Cacheable<SocketUser, RestUser, IUser, ulong> user)
    {
        Target = user;
    }

    internal static SocketUnbanAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
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
        return new SocketUnbanAuditLogData(cacheableUser);
    }

    /// <summary>
    ///     Gets the user that was unbanned.
    /// </summary>
    /// <returns>
    ///     A cacheable user object representing the user that was unbanned.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong> Target { get; }
}
