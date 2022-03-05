namespace Discord
{
    /// <summary>
    ///     Provides the bot/OAuth2 application for an <see cref="IIntegration" />.
    /// </summary>
    public interface IIntegrationApplication
    {
        /// <summary>
        ///     Gets the id of the app.
        /// </summary>
        public ulong Id { get; }
        /// <summary>
        ///     Gets the name of the app.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the icon hash of the app.
        /// </summary>
        public string Icon { get; }
        /// <summary>
        ///     Gets the description of the app.
        /// </summary>
        public string Description { get; }
        /// <summary>
        ///     Gets the summary of the app.
        /// </summary>
        public string Summary { get; }
        /// <summary>
        ///     Gets the bot associated with this application.
        /// </summary>
        public IUser Bot { get; }
    }
}
