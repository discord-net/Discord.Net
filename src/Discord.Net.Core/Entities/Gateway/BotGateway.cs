namespace Discord
{
    /// <summary>
    ///     Stores the gateway information related to the current bot.
    /// </summary>
    public class BotGateway
    {
        /// <summary>
        ///     Gets the WSS URL that can be used for connecting to the gateway.
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        ///     Gets the recommended number of shards to use when connecting.
        /// </summary>
        public int Shards { get; internal set; }
        /// <summary>
        ///     Gets the <see cref="SessionStartLimit"/> that contains the information
        ///     about the current session start limit.
        /// </summary>
        public SessionStartLimit SessionStartLimit { get; internal set; }
    }
}
