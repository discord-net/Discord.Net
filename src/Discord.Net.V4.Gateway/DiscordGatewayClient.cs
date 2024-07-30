using Discord.Gateway.Cache;
using Discord.Gateway.Events;
using Discord.Gateway.State;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Channels;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient : IDiscordClient
{
    public ISelfUserActor CurrentUser => throw new NotImplementedException();

    public IPagedIndexableActor<IGuildActor, ulong, IGuild, IPartialGuild, PageUserGuildsParams> Guilds => throw new NotImplementedException();

    public IIndexableActor<IChannelActor, ulong, IChannel> Channels => throw new NotImplementedException();

    public IIndexableActor<IUserActor, ulong, IUser> Users => throw new NotImplementedException();

    public IIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();

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
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }

    IRestApiClient IDiscordClient.RestApiClient => RestApiClient;
    RequestOptions IDiscordClient.DefaultRequestOptions => DefaultRequestOptions;
}
