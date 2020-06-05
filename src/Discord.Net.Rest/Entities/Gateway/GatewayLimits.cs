using System;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains the rate limits for the gateway.
    /// </summary>
    public class GatewayLimits
    {
        /// <summary>
        ///     Creates a new <see cref="GatewayLimits"/> with the default values.
        /// </summary>
        public static GatewayLimits Default => new GatewayLimits();

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
        ///     Gets or sets the name of the master <see cref="System.Threading.Semaphore"/>
        ///     used by identify.
        /// </summary>
        /// <remarks>
        ///     It is used to define what slave <see cref="System.Threading.Semaphore"/>
        ///     is free to run for concurrent identify requests.
        /// </remarks>
        public string IdentifyMasterSemaphoreName { get; set; }

        /// <summary>
        ///      Gets or sets the name of the slave <see cref="System.Threading.Semaphore"/>
        ///     used by identify.
        /// </summary>
        /// <remarks>
        ///     If the maximum concurrency is higher than one and you are using the sharded client,
        ///     it will be dinamilly renamed to fit the necessary needs.
        /// </remarks>
        public string IdentifySemaphoreName { get; set; }

        /// <summary>
        ///     Gets or sets the maximum identify concurrency.
        /// </summary>
        /// <remarks>
        ///     This limit is provided by Discord.
        /// </remarks>
        public int IdentifyMaxConcurrency { get; set; }

        /// <summary>
        ///     Initializes a new <see cref="GatewayLimits"/> with the default values.
        /// </summary>
        public GatewayLimits()
        {
            Global = new GatewayLimit(120, 60);
            Identify = new GatewayLimit(1, 5);
            PresenceUpdate = new GatewayLimit(5, 60);
            IdentifyMasterSemaphoreName = Guid.NewGuid().ToString();
            IdentifySemaphoreName = Guid.NewGuid().ToString();
            IdentifyMaxConcurrency = 1;
        }

        internal GatewayLimits(GatewayLimits limits)
        {
            Global = new GatewayLimit(limits.Global.Count, limits.Global.Seconds);
            Identify = new GatewayLimit(limits.Identify.Count, limits.Identify.Seconds);
            PresenceUpdate = new GatewayLimit(limits.PresenceUpdate.Count, limits.PresenceUpdate.Seconds);
            IdentifyMasterSemaphoreName = limits.IdentifyMasterSemaphoreName;
            IdentifySemaphoreName = limits.IdentifySemaphoreName;
            IdentifyMaxConcurrency = limits.IdentifyMaxConcurrency;
        }
        

        internal static GatewayLimits GetOrCreate(GatewayLimits limits)
            => limits ?? Default;

        public GatewayLimits Clone()
            => new GatewayLimits(this);
    }
}
