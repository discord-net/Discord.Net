using Discord.Audio;
using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Rest;
using System;

namespace Discord.WebSocket
{
    public class DiscordSocketConfig : DiscordRestConfig
    {
        public const string GatewayEncoding = "json";

        /// <summary> Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting. </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary> Gets or sets the id for this shard. Must be less than TotalShards. </summary>
        public int? ShardId { get; set; } = null;
        /// <summary> Gets or sets the total number of shards for this application. </summary>
        public int? TotalShards { get; set; } = null;

        /// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
        public int MessageCacheSize { get; set; } = 0;
        /// <summary> 
        /// Gets or sets the max number of users a guild may have for offline users to be included in the READY packet. Max is 250.
        /// </summary>
        public int LargeThreshold { get; set; } = 250;

        /// <summary> Gets or sets the type of audio this DiscordClient supports. </summary>
        public AudioMode AudioMode { get; set; } = AudioMode.Disabled;

        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; }
        /// <summary> Gets or sets the provider used to generate new udp sockets. </summary>
        public UdpSocketProvider UdpSocketProvider { get; set; }

        /// <summary> Gets or sets whether or not all users should be downloaded as guilds come available. </summary>
        public bool AlwaysDownloadUsers { get; set; } = false;

        public DiscordSocketConfig()
        {
#if NETSTANDARD1_3
            WebSocketProvider = () => 
            {
                try
                {
                    return new DefaultWebSocketClient();                    
                }
                catch (PlatformNotSupportedException ex)
                {
                    throw new PlatformNotSupportedException("The default websocket provider is not supported on this platform.", ex);
                }
            };
            UdpSocketProvider = () => 
            {
                try
                {
                    return new DefaultUdpSocket();
                }
                catch (PlatformNotSupportedException ex)
                {
                    throw new PlatformNotSupportedException("The default UDP provider is not supported on this platform.", ex);
                }
            };
#else
            WebSocketProvider = () =>
            {
                throw new PlatformNotSupportedException("The default websocket provider is not supported on this platform.\n" +
                    "You must specify a WebSocketProvider or target a runtime supporting .NET Standard 1.3, such as .NET Framework 4.6+.");
            };
            UdpSocketProvider = () =>
            {
                throw new PlatformNotSupportedException("The default UDP provider is not supported on this platform.\n" +
                    "You must specify a UdpSocketProvider or target a runtime supporting .NET Standard 1.3, such as .NET Framework 4.6+.");
            };
#endif
        }

        internal DiscordSocketConfig Clone() => MemberwiseClone() as DiscordSocketConfig;
    }
}
