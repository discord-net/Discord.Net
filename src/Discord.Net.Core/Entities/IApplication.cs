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
        string IconUrl  { get; }

        /// <summary>
        ///     Gets the partial user object containing info on the owner of the application.
        /// </summary>
        IUser Owner { get; }
    }
}
