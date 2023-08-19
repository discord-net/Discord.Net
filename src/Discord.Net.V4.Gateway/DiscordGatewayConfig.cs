using Discord.Gateway.Cache;
using Discord.Rest;
using System;
namespace Discord.Gateway
{
    public delegate IGatewayConnection GatewayConnectionFactory(
        DiscordGatewayClient client,
        DiscordGatewayConfig config
    );

    public sealed class DiscordGatewayConfig : DiscordRestConfig
    {
        public string? CustomGatewayUrl { get; set; }

        public sbyte GatewayVersion { get; set; } = 10;

        public ICacheProvider CacheProvider { get; set; } = new ConcurrentCacheProvider();
        public GatewayConnectionFactory GatewayConnection { get; set; } = WebSocketGatewayConnection.Factory;
    }
}

