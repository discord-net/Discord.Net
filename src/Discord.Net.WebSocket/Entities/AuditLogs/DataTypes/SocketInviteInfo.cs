using Model = Discord.API.AuditLogs.InviteInfoAuditLogModel;

namespace Discord.WebSocket;

/// <summary>
///     Represents information for an invite.
/// </summary>
public struct SocketInviteInfo
{
    internal SocketInviteInfo(Model model)
    {
        MaxAge = model.MaxAge;
        Code = model.Code;
        Temporary = model.Temporary;
        ChannelId = model.ChannelId;
        MaxUses = model.MaxUses;
        CreatorId = model.InviterId;
    }

    /// <summary>
    ///     Gets the time (in seconds) until the invite expires.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the time in seconds until this invite expires; <see langword="null" /> if this
    ///     invite never expires or not specified.
    /// </returns>
    public int? MaxAge { get; }

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
    ///     <see langword="true" /> if users accepting this invite will be removed from the guild when they log off, 
    ///     <see langword="false" /> if not; <see langword="null" /> if not specified.
    /// </returns>
    public bool? Temporary { get; }

    /// <summary>
    ///     Gets the ID of the channel this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the channel snowflake identifier that the invite points to; 
    ///     <see langword="null" /> if not specified.
    /// </returns>
    public ulong? ChannelId { get; }

    /// <summary>
    ///     Gets the max number of uses this invite may have.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of uses this invite may be accepted until it is removed
    ///     from the guild; <see langword="null" /> if none is specified.
    /// </returns>
    public int? MaxUses { get; }

    /// <summary>
    ///     Gets the id of the user created this invite.
    /// </summary>
    public ulong? CreatorId { get; }
}
