using Discord.Rest;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a kick.
/// </summary>
public class SocketKickAuditLogData : ISocketAuditLogData
{
    private SocketKickAuditLogData(Cacheable<SocketUser, RestUser, IUser, ulong> user, string integrationType)
    {
        Target = user;
        IntegrationType = integrationType;
    }

    internal static SocketKickAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var cachedUser = discord.GetUser(entry.TargetId!.Value);
        var cacheableUser = new Cacheable<SocketUser, RestUser, IUser, ulong>(
            cachedUser,
            entry.TargetId.Value,
            cachedUser is not null,
            async () =>
            {
                var user = await discord.ApiClient.GetUserAsync(entry.TargetId!.Value);
                return user is not null ? RestUser.Create(discord, user) : null;
            });
        return new SocketKickAuditLogData(cacheableUser, entry.Options?.IntegrationType);
    }

    /// <summary>
    ///     Gets the user that was kicked.
    /// </summary>
    /// <remarks>
    ///     Download method may return <see langword="null"/> if the user is a 'Deleted User#....'
    ///     because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A cacheable user object representing the kicked user.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong> Target { get; }

    /// <summary>
    ///     Gets the type of integration which performed the action. <see langword="null"/> if the action was performed by a user.
    /// </summary>
    public string IntegrationType { get; }
}
