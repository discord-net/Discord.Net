using Discord.Models;
using Discord.Rest.Actors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Discord.Rest;

public sealed class DiscordRestClient : IDiscordClient
{
    public ApiClient ApiClient { get; }

    public IWebhookActor Webhook(ulong id, string? token = null) => throw new NotImplementedException();

    public DiscordConfig Config { get; }

    public RequestOptions DefaultRequestOptions { get; set; } = new();

    internal ThreadMemberIdentity CurrentUserThreadMemberIdentity { get; }

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

        CurrentUserThreadMemberIdentity = ThreadMemberIdentity.Of(SelfUser.Id);
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
    public IMessage CreateEntity(IMessageModel model) => throw new NotImplementedException();
    public IPartialGuild CreateEntity(IPartialGuildModel model) => throw new NotImplementedException();

    public IGuildMember CreateEntity(IMemberModel model) => throw new NotImplementedException();

    public IDMChannel CreateEntity(IDMChannelModel model) => throw new NotImplementedException();

    public IStageInstance CreateEntity(IStageInstanceModel model) => throw new NotImplementedException();

    public IGuildChannel CreateEntity(IGuildChannelModel model) => throw new NotImplementedException();

    public IInvite CreateEntity(IInviteModel model) => throw new NotImplementedException();

    public IUser CreateEntity(IUserModel model) => throw new NotImplementedException();

    #endregion

    public ISelfUserActor SelfUser => throw new NotImplementedException();

    public IPagedIndexableActor<IGuildActor, ulong, IGuild, IPartialGuild, PageUserGuildsParams> Guilds => throw new NotImplementedException();

    public IIndexableActor<IChannelActor, ulong, IChannel> Channels => throw new NotImplementedException();

    public IIndexableActor<IUserActor, ulong, IUser> Users => throw new NotImplementedException();

    IRestApiClient IDiscordClient.RestApiClient => ApiClient;
    IDiscordClient IClientProvider.Client => this;
}
