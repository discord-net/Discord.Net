namespace Discord.Rest
{
    /// <summary>
    ///     Represents the limits for a gateway request.
    /// </summary>
    public struct GatewayLimit
    {
        /// <summary>
        ///     Gets or sets the maximum amount of this type of request in a time window, that is set by <see cref="Seconds"/>.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        ///     Gets or sets the amount of seconds until the rate limiter resets the remaining requests <see cref="Count"/>.
        /// </summary>
        public int Seconds { get; set; }

        internal GatewayLimit(int count, int seconds)
        {
            Count = count;
            Seconds = seconds;
        }
    }
}
