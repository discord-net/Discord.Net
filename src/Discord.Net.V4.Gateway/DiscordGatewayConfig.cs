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
    
    public GatewayConfiguredObject(Func<T> factory)
    {
        _factory = (_, _) => factory();
        _value = default;
    }

    public T Get(DiscordGatewayClient client, bool cache = true)
    {
        if (_value is null && _factory is null)
            throw new NullReferenceException($"One of '{nameof(_factory)}' or {nameof(_value)} must be set");

        return cache ? _value ??= _factory!(client, client.Config) : _value ?? _factory!(client, client.Config);
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
    public Optional<int> LargeThreshold
    {
        get => _largeThreshold;
        set
        {
            if (!value.IsSpecified)
                _largeThreshold = value;
            
            if(value.Value is > 250 or < 50)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be between 50 and 250");

            _largeThreshold = value;
        }
    }

    public Optional<bool> UsePayloadCompression
    {
        get => _usePayloadCompression;
        set
        {
            if (value is {IsSpecified: true, Value: true})
                _transportCompression = null;

            _usePayloadCompression = value;
        }
    }

    public GatewayIntents Intents { get; set; } = GatewayIntents.AllUnprivileged;

    public string? CustomGatewayUrl { get; set; }

    public sbyte GatewayVersion { get; set; } = 9;

    public bool CreateStoreForEveryEntity { get; set; } = false;
    public ImmutableHashSet<Type> ExtendedStoreTypes { get; set; } = ImmutableHashSet<Type>.Empty;

    public GatewayConfiguredObject<ICacheProvider> CacheProvider { get; set; } = new(ConcurrentCacheProvider.Factory);
    public GatewayConfiguredObject<IGatewayConnection> GatewayConnection { get; set; } = new(WebSocketGatewayConnection.Factory);
    public GatewayConfiguredObject<IGatewayEncoding> Encoding { get; set; } = new(JsonEncoding.Factory);

    public GatewayConfiguredObject<IGatewayCompression>? TransportCompression
    {
        get => _transportCompression;
        set
        {
            if (value is not null)
                _usePayloadCompression = Optional<bool>.Unspecified;

            _transportCompression = value;
        }
    }

    public GatewayConfiguredObject<IGatewayDispatchQueue> DispatchQueue { get; set; } =
        new(ConcurrentGatewayDispatchQueue.Factory);

    public GatewayConfiguredObject<IEventDispatcher> DefaultEventDispatcher { get; set; } =
        LegacyEventDispatcher.Instance;

    public ImmutableDictionary<string, IEventDispatcher> EventDispatchers { get; set; }
        = ImmutableDictionary<string, IEventDispatcher>.Empty;

    public ImmutableArray<GatewayConfiguredObject<IDispatchProcessor>> EventProcessors { get; set; }
        = ImmutableArray<GatewayConfiguredObject<IDispatchProcessor>>.Empty;

    public ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;

    public byte MaxUnacknowledgedHeartbeats { get; set; } = 3;

    private Optional<int> _largeThreshold;
    private GatewayConfiguredObject<IGatewayCompression>? _transportCompression = new(() => new ZLibCompression());
    private Optional<bool> _usePayloadCompression;
    
    public DiscordGatewayConfig(DiscordToken token)
        : base(token)
    { }
}
