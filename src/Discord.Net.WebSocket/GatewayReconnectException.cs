using System;

namespace Discord.WebSocket
{
    /// <summary>
    /// An exception thrown when the gateway client has been requested to
    /// reconnect.
    /// </summary>
    public class GatewayReconnectException : Exception
    {
        /// <summary>
        /// Creates a new instance of the
        /// <see cref="GatewayReconnectException"/> type.
        /// </summary>
        /// <param name="message">
        /// The reason why the gateway has been requested to reconnect.
        /// </param>
        public GatewayReconnectException(string message)
            : base(message)
        { }
    }
}
