using Discord.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Discord.Rest;

// public RestLoadableEntity<,,,> CurrentUser { get; }
// public ApiClient RestApiClient { get; }
// internal RateLimiter RateLimiter { get; }
//
// internal RequestOptions DefaultRequestOptions { get; }
//
// internal readonly DiscordConfig Config;
//
// internal readonly ILogger Logger;
//
// public DiscordRestClient(DiscordConfig config, ILogger logger)
// {
//     Config = config;
//     Logger = logger;
//     RestApiClient = new ApiClient(this, config.Token);
//     RateLimiter = new RateLimiter();
//     DefaultRequestOptions = new RequestOptions(
//         timeout: DiscordConfig.DefaultRequestTimeout,
//         retryMode: Config.DefaultRetryMode,
//         useSystemClock: false
//     );
//
//     // load the ID from the token
//
//     CurrentUser = RestLoadableEntity<,,,>.Create(
//         this,
//         TokenUtils.GetUserIdFromToken(config.Token.Value),
//         Routes.GetCurrentUser,
//         RestSelfUser.Create
//     );
// }
//
// public void Dispose() => throw new NotImplementedException();
//
// public ValueTask DisposeAsync() => throw new NotImplementedException();
//
// IRestApiClient IDiscordClient.RestApiClient => RestApiClient;
// ILoadableEntity<ulong, ISelfUser> IDiscordClient.SelfUser => CurrentUser;

public sealed class DiscordRestClient : IDiscordClient
{
    public ApiClient ApiClient { get; }

    public DiscordConfig Config { get; }

    public RequestOptions DefaultRequestOptions { get; set; } = new();

    internal RateLimiter RateLimiter { get; }

    internal ILogger<DiscordRestClient> Logger { get; }

    public DiscordRestClient(DiscordToken token)
        : this(new DiscordConfig(token), NullLogger<DiscordRestClient>.Instance)
    { }

    public DiscordRestClient(DiscordConfig config) : this(config, NullLogger<DiscordRestClient>.Instance)
    { }

    public DiscordRestClient(DiscordConfig config, ILogger<DiscordRestClient> logger)
    {
        ApiClient = new(this, config.Token);
        Config = config;
        RateLimiter = new();
        Logger = logger;
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public ValueTask DisposeAsync()
    {
        // TODO release managed resources here

        return ValueTask.CompletedTask;
    }

    #region Entity Providers

    public IPartialGuild CreateEntity(IPartialGuildModel model) => throw new NotImplementedException();

    public IGuildMember CreateEntity(IMemberModel model) => throw new NotImplementedException();

    public IDMChannel CreateEntity(IDMChannelModel model) => throw new NotImplementedException();

    public IStageInstance CreateEntity(IStageInstanceModel model) => throw new NotImplementedException();

    public IGuildChannel CreateEntity(IGuildChannelModel model) => throw new NotImplementedException();

    public IInvite CreateEntity(IInviteModel model) => throw new NotImplementedException();

    public IUser CreateEntity(IUserModel model) => throw new NotImplementedException();

    #endregion



    public ILoadableSelfUserActor<ISelfUser> SelfUser => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableGuildActor<IGuild>, ulong, IGuild, IPartialGuild> Guilds => throw new NotImplementedException();

    public IRootActor<ILoadableChannelActor<IChannel>, ulong, IChannel> Channels => throw new NotImplementedException();

    IRestApiClient IDiscordClient.RestApiClient => ApiClient;
    IDiscordClient IClientProvider.Client => this;
}
