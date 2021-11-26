using System;

namespace Discord.WebSocket
{
    /// <summary>
    ///     The exception thrown when the gateway client has been requested to reconnect.
    /// </summary>
    public class GatewayReconnectException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GatewayReconnectException" /> class with the reconnection
        ///     message.
        /// </summary>
        /// <param name="message">The reason why the gateway has been requested to reconnect.</param>
        public GatewayReconnectException(string message)
            : base(message)
        { }
    }
}
