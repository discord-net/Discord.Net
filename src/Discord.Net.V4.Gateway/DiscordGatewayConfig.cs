using Discord.Gateway.Cache;
using Discord.Gateway.Events;
using Discord.Rest;
using System;
using System.Buffers;

namespace Discord.Gateway;

// public delegate IGatewayConnection GatewayConnectionFactory(
//     DiscordGatewayClient client,
//     DiscordGatewayConfig config
// );
//
// public delegate IGatewayEncoding GatewayEncodingFactory(DiscordGatewayClient client);

public delegate T GatewayConfiguredObject<out T>(DiscordGatewayClient client, DiscordGatewayConfig config);

public sealed class DiscordGatewayConfig : DiscordConfig
{
    public GatewayIntents Intents { get; set; }

    public string? CustomGatewayUrl { get; set; }

    public sbyte GatewayVersion { get; set; } = 10;

    public GatewayConfiguredObject<ICacheProvider> CacheProvider { get; set; } = ConcurrentCacheProvider.Factory;
    public GatewayConfiguredObject<IGatewayConnection> GatewayConnection { get; set; } = WebSocketGatewayConnection.Factory;
    public GatewayConfiguredObject<IGatewayEncoding> Encoding { get; set; } = JsonEncoding.Factory;

    public GatewayConfiguredObject<IGatewayDispatchQueue> DispatchQueue { get; set; } =
        ConcurrentGatewayDispatchQueue.Factory;
    public ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;

    public int MaxClientMessageTimeout { get; set; } = 120000;
    public byte MaxUnacknowledgedHeartbeats { get; set; } = 3;

    public DiscordGatewayConfig(DiscordToken token)
        : base(token)
    { }
}
