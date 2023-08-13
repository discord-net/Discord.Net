using System;

namespace Discord
{
    /// <summary>
    ///     Represents a generic invite object.
    /// </summary>
    public interface IInvite : IEntity<string>, IDeletable
    {
        /// <summary>
        ///     Gets the unique identifier for this invite.
        /// </summary>
        /// <returns>
        ///     A string containing the invite code (e.g. <c>FTqNnyS</c>).
        /// </returns>
        string Code { get; }
        /// <summary>
        ///     Gets the URL used to accept this invite using <see cref="Code"/>.
        /// </summary>
        /// <returns>
        ///     A string containing the full invite URL (e.g. <c>https://discord.gg/FTqNnyS</c>).
        /// </returns>
        string Url { get; }

        /// <summary>
        ///     Gets the user that created this invite.
        /// </summary>
        /// <returns>
        ///     A user that created this invite.
        /// </returns>
        IEntitySource<IUser, ulong> Inviter { get; }
        /// <summary>
        ///     Gets the channel this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A generic channel that the invite points to.
        /// </returns>
        IEntitySource<IChannel, ulong> Channel { get; }
        /// <summary>
        ///     Gets the type of the channel this invite is linked to.
        /// </summary>
        ChannelType ChannelType { get; }

        /// <summary>
        ///     Gets the name of the channel this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the channel that the invite points to.
        /// </returns>
        string ChannelName { get; }

        /// <summary>
        ///     Gets the guild this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A guild object representing the guild that the invite points to.
        /// </returns>
        IEntitySource<IGuild, ulong> Guild { get; }

        /// <summary>
        ///     Gets the name of the guild this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the guild that the invite points to.
        /// </returns>
        string GuildName { get; }
        /// <summary>
        ///     Gets the approximated count of online members in the guild.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Int32" /> representing the approximated online member count of the guild that the
        ///     invite points to; <see langword="null" /> if one cannot be obtained.
        /// </returns>
        int? PresenceCount { get; }
        /// <summary>
        ///     Gets the approximated count of total members in the guild.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Int32" /> representing the approximated total member count of the guild that the
        ///     invite points to; <see langword="null" /> if one cannot be obtained.
        /// </returns>
        int? MemberCount { get; }
        /// <summary>
        ///     Gets the user this invite is linked to via <see cref="InviteTargetType"/>.
        /// </summary>
        /// <returns>
        ///     A user that is linked to this invite.
        /// </returns>
        IEntitySource<IUser, ulong>? TargetUser { get; }
        /// <summary>
        ///     Gets the type of the linked <see cref="TargetUser"/> for this invite.
        /// </summary>
        /// <returns>
        ///     The type of the linked user that is linked to this invite.
        /// </returns>
        InviteTargetType InviteTargetType { get; }

        /// <summary>
        ///     Gets the embedded application to open for this voice channel embedded application invite.
        /// </summary>
        /// <returns>
        ///     A partial <see cref="IApplication"/> object. <see langword="null" /> if <see cref="InviteTargetType"/>
        ///     is not <see cref="InviteTargetType.EmbeddedApplication"/>.
        /// </returns>
        IApplication Application { get; }

        /// <summary>
        ///     Gets the expiration date of this invite. <see langword="null" /> if the invite never expires.
        /// </summary>
        DateTimeOffset? ExpiresAt { get; }
    }
}
