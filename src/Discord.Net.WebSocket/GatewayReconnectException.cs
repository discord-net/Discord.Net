using System;

namespace Discord.WebSocket
{
    /// <summary>
    /// An exception thrown when Discord requests the gateway client to
    /// reconnect.
    /// </summary>
    public class GatewayReconnectException : Exception
    {
        /// <summary>
        /// Creates a new instance of the
        /// <see cref="GatewayReconnectException"/> type.
        /// </summary>
        public GatewayReconnectException()
            : base("Server requested a reconnect")
        { }
    }
}
