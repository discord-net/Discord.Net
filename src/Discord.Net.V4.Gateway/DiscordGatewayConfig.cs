using Discord.Gateway.Cache;
using Discord.Rest;
using System;
namespace Discord.Gateway
{
    public sealed class DiscordGatewayConfig : DiscordRestConfig
    {
        public string? CustomGatewayUrl { get; set; }

        public ICacheProvider CacheProvider { get; set; } = new ConcurrentCacheProvider();
        public IGatewayConnection GatewayConnection { get; set; } = new WebSocketGatewayConnection();
    }
}

