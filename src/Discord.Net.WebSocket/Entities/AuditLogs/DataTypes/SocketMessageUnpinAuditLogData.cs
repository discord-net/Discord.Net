using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an unpinned message.
/// </summary>
public class SocketMessageUnpinAuditLogData : ISocketAuditLogData
{
    private SocketMessageUnpinAuditLogData(ulong messageId, ulong channelId, Cacheable<SocketUser, RestUser, IUser, ulong>? user)
    {
        MessageId = messageId;
        ChannelId = channelId;
        Target = user;
    }

    internal static SocketMessageUnpinAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        Cacheable<SocketUser, RestUser, IUser, ulong>? cacheableUser = null;
        if (entry.TargetId.HasValue)
        {
            var cachedUser = discord.GetUser(entry.TargetId.Value);
            cacheableUser = new Cacheable<SocketUser, RestUser, IUser, ulong>(
                cachedUser,
                entry.TargetId.Value,
                cachedUser is not null,
                async () =>
                {
                    var user = await discord.ApiClient.GetUserAsync(entry.TargetId.Value);
                    return user is not null ? RestUser.Create(discord, user) : null;
                });
        }

        return new SocketMessageUnpinAuditLogData(entry.Options.MessageId!.Value, entry.Options.ChannelId!.Value, cacheableUser);
    }

    /// <summary>
    ///     Gets the ID of the messages that was unpinned.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the messages that was unpinned.
    /// </returns>
    public ulong MessageId { get; }

    /// <summary>
    ///     Gets the ID of the channel that the message was unpinned from.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the message was unpinned from.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the user of the message that was unpinned if available.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A user object representing the user that created the unpinned message or <see langword="null"/>.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong>? Target { get; }
}
