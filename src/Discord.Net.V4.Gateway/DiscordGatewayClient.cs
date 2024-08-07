using Discord.Gateway;
using Discord.Gateway.Events;
using Discord.Gateway.State;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Channels;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient : IDiscordClient
{
    public ICurrentUserActor CurrentUser => throw new NotImplementedException();

    public GatewayRequestOptions DefaultRequestOptions { get; }

    public DiscordRestClient Rest { get; }

    internal RestApiClient RestApiClient => Rest.RestApiClient;

    [SourceOfTruth]
    internal DiscordGatewayConfig Config { get; }

    internal ILoggerFactory LoggerFactory { get; }

    internal IGatewayEncoding Encoding { get; }

    internal ICacheProvider CacheProvider { get; }

    internal StateController StateController { get; private set; }

    public DiscordGatewayClient(
        DiscordGatewayConfig config,
        ILoggerFactory? loggerFactory = null)
    {
        Config = config;

        DefaultRequestOptions = new();

        LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        Rest = new DiscordRestClient(config, LoggerFactory.CreateLogger<DiscordRestClient>());
        Encoding = config.Encoding(this, Config);
        CacheProvider = config.CacheProvider(this, config);

        _heartbeatSignal = Channel.CreateBounded<HeartbeatSignal>(
            new BoundedChannelOptions(2)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = false
            },
            HandleHeartbeatSignalDropped
        );

        _dispatchQueue = Config.DispatchQueue(this, Config);

        StateController = new(this, LoggerFactory.CreateLogger<StateController>());

        Channels = new(id => new GatewayChannelActor(this, ChannelIdentity.Of(id)));
        Guilds = GatewayActors.PageGuilds(this);
        Users = new(id => new GatewayUserActor(this, UserIdentity.Of(id)));
    }

    public ValueTask DisposeAsync()
    {
        // TODO release managed resources here
        return ValueTask.CompletedTask;
    }

    IRestApiClient IDiscordClient.RestApiClient => RestApiClient;
    RequestOptions IDiscordClient.DefaultRequestOptions => DefaultRequestOptions;
}
