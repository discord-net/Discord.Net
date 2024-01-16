using Discord.API.AuditLogs;
using Discord.Rest;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an invite removal.
/// </summary>
public class SocketInviteDeleteAuditLogData : ISocketAuditLogData
{
    private SocketInviteDeleteAuditLogData(InviteInfoAuditLogModel model, Cacheable<SocketUser, RestUser, IUser, ulong>? inviter)
    {
        MaxAge = model.MaxAge!.Value;
        Code = model.Code;
        Temporary = model.Temporary!.Value;
        Creator = inviter;
        ChannelId = model.ChannelId!.Value;
        Uses = model.Uses!.Value;
        MaxUses = model.MaxUses!.Value;
    }

    internal static SocketInviteDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<InviteInfoAuditLogModel>(changes, discord);

        Cacheable<SocketUser, RestUser, IUser, ulong>? cacheableUser = null;

        if (data.InviterId != null)
        {
            var cachedUser = discord.GetUser(data.InviterId.Value);
            cacheableUser = new Cacheable<SocketUser, RestUser, IUser, ulong>(
                cachedUser,
                data.InviterId.Value,
                cachedUser is not null,
                async () =>
                {
                    var user = await discord.ApiClient.GetUserAsync(data.InviterId.Value);
                    return user is not null ? RestUser.Create(discord, user) : null;
                });
        }

        return new SocketInviteDeleteAuditLogData(data, cacheableUser);
    }

    /// <summary>
    ///     Gets the time (in seconds) until the invite expires.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the time in seconds until this invite expires.
    /// </returns>
    public int MaxAge { get; }

    /// <summary>
    ///     Gets the unique identifier for this invite.
    /// </summary>
    /// <returns>
    ///     A string containing the invite code (e.g. <c>FTqNnyS</c>).
    /// </returns>
    public string Code { get; }

    /// <summary>
    ///     Gets a value that indicates whether the invite is a temporary one.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if users accepting this invite will be removed from the guild when they log off; otherwise
    ///     <see langword="false" />.
    /// </returns>
    public bool Temporary { get; }

    /// <summary>
    ///     Gets the user that created this invite if available.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A user that created this invite or <see langword="null"/>.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong>? Creator { get; }

    /// <summary>
    ///     Gets the ID of the channel this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the channel snowflake identifier that the invite points to.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the number of times this invite has been used.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of times this invite has been used.
    /// </returns>
    public int Uses { get; }

    /// <summary>
    ///     Gets the max number of uses this invite may have.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of uses this invite may be accepted until it is removed
    ///     from the guild; <see langword="null" /> if none is set.
    /// </returns>
    public int MaxUses { get; }
}
