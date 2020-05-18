namespace Discord.Rest
{
    /// <summary>
    ///     Contains the rate limits for the gateway.
    /// </summary>
    public class GatewayLimits
    {
        /// <summary>
        ///     Gets or sets the global limits for the gateway rate limiter.
        /// </summary>
        /// <remarks>
        ///     It includes all the other limits, like Identify.
        /// </remarks>
        public GatewayLimit Global { get; set; }
        /// <summary>
        ///     Gets or sets the limits of Identify requests.
        /// </summary>
        public GatewayLimit Identify { get; set; }

        public GatewayLimits()
        {
            Global = new GatewayLimit(120, 60);
            Identify = new GatewayLimit(1, 5);
        }

        internal static GatewayLimits GetOrCreate(GatewayLimits limits)
            => limits ?? new GatewayLimits();
    }
}
