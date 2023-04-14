using Discord.Rest;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a adding a bot to a guild.
/// </summary>
public class SocketBotAddAuditLogData : ISocketAuditLogData
{
    private SocketBotAddAuditLogData(Cacheable<SocketUser, RestUser, IUser, ulong> bot)
    {
        Target = bot;
    }

    internal static SocketBotAddAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
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
        return new SocketBotAddAuditLogData(cacheableUser);
    }

    /// <summary>
    ///     Gets the bot that was added.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the bot is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A cacheable user object representing the bot.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong> Target { get; }
}
