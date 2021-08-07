namespace Discord
{
    /// <summary>
    ///     Represents a Discord application created via the developer portal.
    /// </summary>
    public interface IApplication : ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the name of the application.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Gets the description of the application.
        /// </summary>
        string Description { get; }
        /// <summary>
        ///     Gets the RPC origins of the application.
        /// </summary>
        string[] RPCOrigins { get; }
        ulong Flags { get; }
        /// <summary>
        ///     Gets the icon URL of the application.
        /// </summary>
        string IconUrl { get; }
        /// <summary>
        ///     Gets if the bot is public.
        /// </summary>
        bool IsBotPublic { get; }
        /// <summary>
        ///     Gets if the bot requires code grant.
        /// </summary>
        bool BotRequiresCodeGrant { get; }
        /// <summary>
        ///     Gets the team associated with this application if there is one.
        /// </summary>
        ITeam Team { get; }

        /// <summary>
        ///     Gets the partial user object containing info on the owner of the application.
        /// </summary>
        IUser Owner { get; }

        /// <summary>
        ///     Get the icon URL for this Application.
        /// </summary>
        /// <remarks>
        ///     This property retrieves a URL for this Application's icon. In event that the application does not have a valid icon
        ///     (i.e. their icon identifier is not set), this property will return <c>null</c>.
        /// </remarks>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <returns>A string representing the application's icon URL; <c>null</c> if the application does not have an icon in place.</returns>
        string GetIconUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);
    }
}
