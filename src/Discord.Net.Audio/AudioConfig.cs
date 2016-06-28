using Discord.Net.WebSockets;

namespace Discord.Audio
{
    public class AudioConfig
    {
        /// <summary> Gets or sets the time (in milliseconds) to wait for the websocket to connect and initialize. </summary>
        public int ConnectionTimeout { get; set; } = 30000;
        /// <summary> Gets or sets the time (in milliseconds) to wait after an unexpected disconnect before reconnecting. </summary>
        public int ReconnectDelay { get; set; } = 1000;
        /// <summary> Gets or sets the time (in milliseconds) to wait after an reconnect fails before retrying. </summary>
        public int FailedReconnectDelay { get; set; } = 15000;

        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; } = () => new DefaultWebSocketClient();
    }
}
