using Discord.Gateway.Cache;
using Discord.Rest;
using System;
using System.Buffers;

namespace Discord.Gateway
{
    public delegate IGatewayConnection GatewayConnectionFactory(
        DiscordGatewayClient client,
        DiscordGatewayConfig config
    );

    public sealed class DiscordGatewayConfig : DiscordRestConfig
    {
        public GatewayIntents Intents { get; set; }

        public string? CustomGatewayUrl { get; set; }

        public sbyte GatewayVersion { get; set; } = 10;

        public ICacheProvider CacheProvider { get; set; } = new ConcurrentCacheProvider();
        public GatewayConnectionFactory GatewayConnection { get; set; } = WebSocketGatewayConnection.Factory;
        public IGatewayEncoding Encoding { get; set; }
        public ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;

        public int MaxClientMessageTimeout { get; set; } = 120000;
        public byte MaxUnacknowledgedHeartbeats { get; set; } = 3;

        public DiscordGatewayConfig(DiscordToken token)
            : base(token)
        {
            Encoding = new ETFEncoding(this);
        }
    }
}

