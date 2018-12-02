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
        ///     Gets the channel this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A generic channel that the invite points to.
        /// </returns>
        IChannel Channel { get; }
        /// <summary> Gets the type of the channel this invite is linked to. </summary>
        ChannelType ChannelType { get; }
        /// <summary>
        ///     Gets the ID of the channel this invite is linked to.
        /// </summary>
        /// <returns>
        ///     An <see cref="ulong"/> representing the channel snowflake identifier that the invite points to.
        /// </returns>
        ulong ChannelId { get; }
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
        IGuild Guild { get; }
        /// <summary>
        ///     Gets the ID of the guild this invite is linked to.
        /// </summary>
        /// <returns>
        ///     An <see cref="ulong"/> representing the guild snowflake identifier that the invite points to.
        /// </returns>
        ulong? GuildId { get; }
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
        ///     invite points to; <c>null</c> if one cannot be obtained.
        /// </returns>
        int? PresenceCount { get; }
        /// <summary>
        ///     Gets the approximated count of total members in the guild.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Int32" /> representing the approximated total member count of the guild that the
        ///     invite points to; <c>null</c> if one cannot be obtained.
        /// </returns>
        int? MemberCount { get; }
    }
}
