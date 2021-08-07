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
        ///     Get the icon URL for this UserGuild.
        /// </summary>
        /// <remarks>
        ///     This property retrieves a URL for this UserGuild's icon. In event that the user guild does not have a valid icon
        ///     (i.e. their icon identifier is not set), this property will return <c>null</c>.
        /// </remarks>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <returns>A string representing the user guild's icon URL; <c>null</c> if the user guild does not have an icon in place.</returns>
        string GetIconUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);
    }
}
