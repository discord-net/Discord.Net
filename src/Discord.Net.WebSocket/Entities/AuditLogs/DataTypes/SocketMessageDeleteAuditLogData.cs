using Discord.Rest;
using System;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to message deletion(s).
/// </summary>
public class SocketMessageDeleteAuditLogData : ISocketAuditLogData
{
    private SocketMessageDeleteAuditLogData(ulong channelId, int count, Cacheable<SocketUser, RestUser, IUser, ulong> user)
    {
        ChannelId = channelId;
        MessageCount = count;
        Target = user;
    }

    internal static SocketMessageDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
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

        return new SocketMessageDeleteAuditLogData(entry.Options.ChannelId!.Value, entry.Options.Count!.Value, cacheableUser);
    }

    /// <summary>
    ///     Gets the number of messages that were deleted.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of messages that were deleted from the channel.
    /// </returns>
    public int MessageCount { get; }

    /// <summary>
    ///     Gets the ID of the channel that the messages were deleted from.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the messages were
    ///     deleted from.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the user of the messages that were deleted.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A user object representing the user that created the deleted messages.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong> Target { get; }
}
