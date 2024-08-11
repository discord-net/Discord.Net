using Discord.Gateway;
using Discord.Gateway.Events;
using Discord.Gateway.Processors;
using Discord.Rest;
using System;
using System.Buffers;
using System.Collections.Immutable;

namespace Discord.Gateway;

public struct GatewayConfiguredObject<T>
{
    public delegate T Factory(DiscordGatewayClient client, DiscordGatewayConfig config);

    private T? _value;
    private readonly Factory? _factory;

    public GatewayConfiguredObject(T value)
    {
        _value = value;
        _factory = null;
    }

    public GatewayConfiguredObject(Factory factory)
    {
        _factory = factory;
        _value = default;
    }

    public GatewayConfiguredObject(Func<DiscordGatewayClient, T> factory)
    {
        _factory = (client, _) => factory(client);
        _value = default;
    }

    public GatewayConfiguredObject(Func<DiscordGatewayConfig, T> factory)
    {
        _factory = (_, config) => factory(config);
        _value = default;
    }

    public T Get(DiscordGatewayClient client)
    {
        if (_value is null && _factory is null)
            throw new NullReferenceException($"One of '{nameof(_factory)}' or {nameof(_value)} must be set");

        return _value ??= _factory!(client, client.Config);
    }

    public static implicit operator GatewayConfiguredObject<T>(T value) => new(value);
    public static implicit operator GatewayConfiguredObject<T>(Factory factory) => new(factory);
    public static implicit operator GatewayConfiguredObject<T>(Func<DiscordGatewayClient, T> factory)
        => new(factory);
    public static implicit operator GatewayConfiguredObject<T>(Func<DiscordGatewayConfig, T> factory)
        => new(factory);
}

public sealed class DiscordGatewayConfig : DiscordConfig
{
    public GatewayIntents Intents { get; set; }

    public string? CustomGatewayUrl { get; set; }

    public sbyte GatewayVersion { get; set; } = 10;

    public bool CreateStoreForEveryEntity { get; set; } = false;
    public ImmutableHashSet<Type> ExtendedStoreTypes { get; set; } = ImmutableHashSet<Type>.Empty;

    public GatewayConfiguredObject<ICacheProvider> CacheProvider { get; set; } = new(ConcurrentCacheProvider.Factory);
    public GatewayConfiguredObject<IGatewayConnection> GatewayConnection { get; set; } = new(WebSocketGatewayConnection.Factory);
    public GatewayConfiguredObject<IGatewayEncoding> Encoding { get; set; } = new(JsonEncoding.Factory);

    public GatewayConfiguredObject<IGatewayCompression>? GatewayCompression { get; set; } =
        new(IGatewayCompression.ZLib);

    public GatewayConfiguredObject<IGatewayDispatchQueue> DispatchQueue { get; set; } =
        new(ConcurrentGatewayDispatchQueue.Factory);

    public GatewayConfiguredObject<IEventDispatcher> DefaultEventDispatcher { get; set; } =
        LegacyEventDispatcher.Instance;

    public ImmutableDictionary<string, IEventDispatcher> EventDispatchers { get; set; }
        = ImmutableDictionary<string, IEventDispatcher>.Empty;

    public ImmutableArray<GatewayConfiguredObject<IDispatchProcessor>> EventProcessors { get; set; }

    public ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;

    public int MaxClientMessageTimeout { get; set; } = 120000;
    public byte MaxUnacknowledgedHeartbeats { get; set; } = 3;

    public DiscordGatewayConfig(DiscordToken token)
        : base(token)
    { }
}
