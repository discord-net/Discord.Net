namespace Discord
{
    public interface IUserGuild : IDeletable, ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the name of this guild.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Gets the icon URL associated with this guild, or <c>null</c> if one is not set.
        /// </summary>
        string IconUrl { get; }
        /// <summary>
        ///     Returns <c>true</c> if the current user owns this guild.
        /// </summary>
        bool IsOwner { get; }
        /// <summary>
        ///     Returns the current user's permissions for this guild.
        /// </summary>
        GuildPermissions Permissions { get; }

        /// <summary>
        ///     Gets the features for this guild.
        /// </summary>
        /// <returns>
        ///     A flags enum containing all the features for the guild.
        /// </returns>
        GuildFeatures Features { get; }

        /// <summary>
        ///     Gets the approximate number of members in this guild.
        /// </summary>
        /// <remarks>
        ///     Only available when getting a guild via REST when `with_counts` is true.
        /// </remarks>
        int? ApproximateMemberCount { get; }

        /// <summary>
        ///     Gets the approximate number of non-offline members in this guild.
        /// </summary>
        /// <remarks>
        ///     Only available when getting a guild via REST when `with_counts` is true.
        /// </remarks>
        int? ApproximatePresenceCount { get; }
    }
}
