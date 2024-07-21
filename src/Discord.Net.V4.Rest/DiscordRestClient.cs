using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Webhooks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Discord.Rest;

public sealed partial class DiscordRestClient : IDiscordClient
{
    [SourceOfTruth]
    public RestSelfUserActor SelfUser { get; }

    public IPagedIndexableActor<IGuildActor, ulong, IGuild, IPartialGuild, PageUserGuildsParams> Guilds => throw new NotImplementedException();

    [SourceOfTruth]
    public RestIndexableActor<RestChannelActor, ulong, RestChannel> Channels { get; }

    [SourceOfTruth]
    public RestIndexableActor<RestUserActor, ulong, RestUser> Users { get; }

    [SourceOfTruth]
    public RestIndexableActor<RestWebhookActor, ulong, RestWebhook> Webhooks { get; }


    [SourceOfTruth]
    public RestApiClient RestApiClient { get; }

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
        RestApiClient = new(this, config.Token);
        Config = config;
        RateLimiter = new();
        Logger = logger;

        SelfUser = new RestSelfUserActor(this, SelfUserIdentity.Of(TokenUtils.GetUserIdFromToken(config.Token.Value)));
        Channels = new(id => new RestChannelActor(this, ChannelIdentity.Of(id)));
        Users = new(id => new RestUserActor(this, UserIdentity.Of(id)));
        Webhooks = new(id => new RestWebhookActor(this, WebhookIdentity.Of(id)));
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
}
