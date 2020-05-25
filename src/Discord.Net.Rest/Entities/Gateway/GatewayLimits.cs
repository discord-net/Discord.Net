using System;

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
        ///     This property includes all the other limits, like Identify,
        ///     and it is per websocket.
        /// </remarks>
        public GatewayLimit Global { get; set; }
        /// <summary>
        ///     Gets or sets the limits of Identify requests.
        /// </summary>
        /// <remarks>
        ///     This limit is included into <see cref="Global"/> but it is
        ///     also per account.
        /// </remarks>
        public GatewayLimit Identify { get; set; }
        /// <summary>
        ///     Gets or sets the limits of Presence Update requests.
        /// </summary>
        /// <remarks>
        ///     Presence updates include activity (playing, watching, etc)
        ///     and status (online, idle, etc)
        /// </remarks>
        public GatewayLimit PresenceUpdate { get; set; }
        /// <summary>
        ///     Gets or sets the name of the <see cref="System.Threading.Semaphore"/> used by identify.
        /// </summary>
        public string IdentifySemaphoreName { get; set; }

        /// <summary>
        ///     Initializes a new <see cref="GatewayLimits"/> with the default values.
        /// </summary>
        public GatewayLimits()
        {
            Global = new GatewayLimit(120, 60);
            Identify = new GatewayLimit(1, 5);
            PresenceUpdate = new GatewayLimit(5, 60);
            IdentifySemaphoreName = Guid.NewGuid().ToString();
        }

        internal static GatewayLimits GetOrCreate(GatewayLimits limits)
            => limits ?? new GatewayLimits();
    }
}
