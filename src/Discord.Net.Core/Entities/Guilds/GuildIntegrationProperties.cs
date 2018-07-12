namespace Discord
{
    /// <summary>
    ///     Provides properties used to modify an <see cref="IGuildIntegration" /> with the specified changes.
    /// </summary>
    public class GuildIntegrationProperties
    {
        /// <summary>
        ///     Gets or sets the behavior when an integration subscription lapses.
        /// </summary>
        public Optional<int> ExpireBehavior { get; set; }
        /// <summary>
        ///     Gets or sets the period (in seconds) where the integration will ignore lapsed subscriptions.
        /// </summary>
        public Optional<int> ExpireGracePeriod { get; set; }
        /// <summary>
        ///     Gets or sets whether emoticons should be synced for this integration.
        /// </summary>
        public Optional<bool> EnableEmoticons { get; set; }
    }
}
