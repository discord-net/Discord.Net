using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Discord.Rest;

using GuildsPager = RestPagedIndexableActor<RestGuildActor, ulong, RestGuild, RestPartialGuild, IEnumerable<IPartialGuildModel>, PageUserGuildsParams>;

public sealed partial class DiscordRestClient : IDiscordClient
{
    //[SourceOfTruth]
    public RestSelfUserActor CurrentUser { get; }

    ISelfUserActor IDiscordClient.CurrentUser => CurrentUser;

    [SourceOfTruth]
    public GuildsPager Guilds { get; }

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
        Config = config;
        Config.JsonSerializerOptions.MakeReadOnly(true);

        RestApiClient = new(this, config.Token);
        RateLimiter = new();
        Logger = logger;

        CurrentUser = new RestSelfUserActor(this, SelfUserIdentity.Of(TokenUtils.GetUserIdFromToken(config.Token.Value)));
        Channels = new(id => new RestChannelActor(this, ChannelIdentity.Of(id)));
        Users = new(id => new RestUserActor(this, UserIdentity.Of(id)));
        Webhooks = new(id => new RestWebhookActor(this, WebhookIdentity.Of(id)));
        CurrentUserThreadMemberIdentity = ThreadMemberIdentity.Of(CurrentUser.Id);
        Guilds = RestActors.PagedGuilds(this);
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
