namespace Discord.Rest
{
    /// <summary>
    ///     Represents the limits for a gateway request.
    /// </summary>
    public struct GatewayLimit
    {
        /// <summary>
        ///     Gets or sets the maximum amount of this type of request in a time frame, that is set by <see cref="Seconds"/>.
        /// </summary>
        /// <returns>
        ///     Returns the maximum amount of this type of request in a time frame to not trigger the rate limit.
        /// </returns>
        public int Count { get; set; }
        /// <summary>
        ///     Gets or sets the amount of seconds until the rate limiter resets the remaining requests back to <see cref="Count"/>.
        /// </summary>
        /// <returns>
        ///     Returns the amount of seconds that define the time frame to reset.
        /// </returns>
        public int Seconds { get; set; }

        internal GatewayLimit(int count, int seconds)
        {
            Count = count;
            Seconds = seconds;
        }
    }
}
