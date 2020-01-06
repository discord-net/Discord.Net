using Discord.Socket;
using Discord.Socket.Providers;
using System;

namespace Discord
{
    public class DiscordConfig
    {
        /// <summary>
        /// Discord.Net version
        /// </summary>
        public const string Version = "3.0.0a0";
        /// <summary>
        /// Discord.Net User-Agent
        /// </summary>
        public const string UserAgent = "DiscordBot (https://github.com/discord-net/Discord.Net, " + Version + ")";

        /// <summary>
        /// The default, fallback Gateway URI. This will generally be replaced by <see cref="Rest.IDiscordRestApi.GetGatewayAsync"/>.
        /// </summary>
        public static readonly Uri DefaultGatewayUri = new Uri("wss://gateway.discord.gg");
        /// <summary>
        /// The base URL for the Rest API.
        /// </summary>
        public string RestApiUrl { get; set; } = "https://discordapp.com/api/v6/";
        /// <summary>
        /// The URI to use when connecting to the gateway. If specified, this will override the URI Discord instructs us to use.
        /// </summary>
        public Uri? GatewayUri = null;

        public SocketFactory SocketFactory { get; set; } = DefaultSocketFactory.Create;
    }
}
