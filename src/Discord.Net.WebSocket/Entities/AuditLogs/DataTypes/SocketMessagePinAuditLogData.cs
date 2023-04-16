using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a pinned message.
/// </summary>
public class SocketMessagePinAuditLogData : ISocketAuditLogData
{
    private SocketMessagePinAuditLogData(ulong messageId, ulong channelId, Cacheable<SocketUser, RestUser, IUser, ulong>? user)
    {
        MessageId = messageId;
        ChannelId = channelId;
        Target = user;
    }

    internal static SocketMessagePinAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
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

        return new SocketMessagePinAuditLogData(entry.Options.MessageId!.Value, entry.Options.ChannelId!.Value, cacheableUser);
    }

    /// <summary>
    ///     Gets the ID of the messages that was pinned.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the messages that was pinned.
    /// </returns>
    public ulong MessageId { get; }

    /// <summary>
    ///     Gets the ID of the channel that the message was pinned from.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the message was pinned from.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the user of the message that was pinned if available.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A user object representing the user that created the pinned message or <see langword="null"/>.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong>? Target { get; }
}
