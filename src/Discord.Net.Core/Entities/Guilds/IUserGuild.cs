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
    }
}
