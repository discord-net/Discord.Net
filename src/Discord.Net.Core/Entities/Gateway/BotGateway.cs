namespace Discord
{
    public class BotGateway
    {
        /// <summary>
        ///     The WSS URL that can be used for connecting to the gateway.
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        ///     The recommended number of shards to use when connecting.
        /// </summary>
        public int Shards { get; internal set; }
        /// <summary>
        ///     Information on the current session start limit.
        /// </summary>
        public SessionStartLimit SessionStartLimit { get; internal set; }
    }
}
