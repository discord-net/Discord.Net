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
        /// <param name="format">The format to return. Mustn't be a gif.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        string GetIconUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);

        /// <summary>
        ///     Gets the partial user object containing info on the owner of the application.
        /// </summary>
        IUser Owner { get; }
    }
}
