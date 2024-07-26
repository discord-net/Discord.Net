using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient : IDiscordClient
{
    public ISelfUserActor CurrentUser => throw new NotImplementedException();

    public IPagedIndexableActor<IGuildActor, ulong, IGuild, IPartialGuild, PageUserGuildsParams> Guilds => throw new NotImplementedException();

    public IIndexableActor<IChannelActor, ulong, IChannel> Channels => throw new NotImplementedException();

    public IIndexableActor<IUserActor, ulong, IUser> Users => throw new NotImplementedException();

    public IIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();

    public RequestOptions DefaultRequestOptions => Rest.DefaultRequestOptions;

    public DiscordRestClient Rest { get; }

    [SourceOfTruth]
    internal DiscordGatewayConfig Config { get; }

    internal ILoggerFactory LoggerFactory { get; }

    internal IGatewayEncoding Encoding { get; }

    public DiscordGatewayClient(
        DiscordGatewayConfig config,
        ILoggerFactory? loggerFactory = null)
    {
        Config = config;

        LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        Rest = new DiscordRestClient(config, LoggerFactory.CreateLogger<DiscordRestClient>());
        Encoding = config.Encoding(this);
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }

    IRestApiClient IDiscordClient.RestApiClient => Rest.RestApiClient;
}
