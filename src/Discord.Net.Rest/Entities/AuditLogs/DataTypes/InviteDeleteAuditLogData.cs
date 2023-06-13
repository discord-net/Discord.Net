using Discord.API.AuditLogs;
using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an invite removal.
/// </summary>
public class InviteDeleteAuditLogData : IAuditLogData
{
    private InviteDeleteAuditLogData(InviteInfoAuditLogModel model, IUser inviter)
    {
        MaxAge = model.MaxAge!.Value;
        Code = model.Code;
        Temporary = model.Temporary!.Value;
        Creator = inviter;
        ChannelId = model.ChannelId!.Value;
        Uses = model.Uses!.Value;
        MaxUses = model.MaxUses!.Value;
    }

    internal static InviteDeleteAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<InviteInfoAuditLogModel>(changes, discord);

        RestUser inviter = null;

        if (data.InviterId != null)
        {
            var inviterInfo = log.Users.FirstOrDefault(x => x.Id == data.InviterId);
            inviter = (inviterInfo != null) ? RestUser.Create(discord, inviterInfo) : null;
        }

        return new InviteDeleteAuditLogData(data, inviter);
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
    ///     <c>true</c> if users accepting this invite will be removed from the guild when they log off; otherwise
    ///     <c>false</c>.
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
    public IUser Creator { get; }

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
    ///     from the guild; <c>null</c> if none is set.
    /// </returns>
    public int MaxUses { get; }
}
