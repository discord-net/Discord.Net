using Discord.Models;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Discord.Rest;

// using GuildsPager = RestPartialPagedIndexableLink<
//     RestGuildActor,
//     ulong,
//     RestGuild,
//     RestPartialGuild,
//     IPartialGuildModel,
//     IEnumerable<IPartialGuildModel>,
//     PageUserGuildsParams
// >;
// using Stickers = RestIndexableLink<RestStickerActor, ulong, RestSticker>;
// using StickerPacks = RestEnumerableIndexableLink<
//     RestStickerPackActor,
//     ulong,
//     RestStickerPack,
//     IStickerPack,
//     IEnumerable<IStickerPackModel>
// >;

public sealed partial class DiscordRestClient : IDiscordClient
{
    [SourceOfTruth]
    public RestCurrentUserActor CurrentUser { get; }
    
    [SourceOfTruth]
    public GuildLink
        .Paged<RestPartialGuild, IPartialGuildModel, PageUserGuildsParams, IEnumerable<IPartialGuildModel>>
        .Indexable Guilds { get; }

    [SourceOfTruth] 
    public ChannelLink.Indexable Channels { get; }

    [SourceOfTruth] public UserLink.Indexable Users { get; }

    [SourceOfTruth] public WebhookLink.Indexable Webhooks { get; }
    [SourceOfTruth] public InviteLink.Indexable Invites { get; }

    [SourceOfTruth] public StickerPackLink.Enumerable.Indexable StickerPacks { get; }

    [SourceOfTruth] public StickerLink.Indexable Stickers { get; }

    [SourceOfTruth] public RestApiClient RestApiClient { get; }

    public DiscordConfig Config { get; }

    public RequestOptions DefaultRequestOptions { get; set; } = new();

    internal ThreadMemberIdentity CurrentUserThreadMemberIdentity { get; }

    internal RateLimiter RateLimiter { get; }

    internal ILogger<DiscordRestClient> Logger { get; }

    public DiscordRestClient(DiscordToken token)
        : this(new DiscordConfig(token), NullLogger<DiscordRestClient>.Instance)
    {
    }

    public DiscordRestClient(DiscordConfig config) : this(config, NullLogger<DiscordRestClient>.Instance)
    {
    }

    public DiscordRestClient(DiscordConfig config, ILogger<DiscordRestClient> logger)
    {
        Config = config;
        Config.JsonSerializerOptions.MakeReadOnly(true);

        RestApiClient = new(this, config.Token);
        RateLimiter = new();
        Logger = logger;

        CurrentUser =
            new RestCurrentUserActor(this, SelfUserIdentity.Of(TokenUtils.GetUserIdFromToken(config.Token.Value)));
        Channels = new(this, id => new RestChannelActor(this, ChannelIdentity.Of(id)));
        Users = new(this, id => new RestUserActor(this, UserIdentity.Of(id)));
        Webhooks = new(this, id => new RestWebhookActor(this, WebhookIdentity.Of(id)));
        CurrentUserThreadMemberIdentity = ThreadMemberIdentity.Of(CurrentUser.Id);
        Guilds = RestActors.PagedGuilds(this);
        StickerPacks = RestActors.Fetchable(
            Template.T<RestStickerPackActor>(),
            this,
            RestStickerPackActor.Factory,
            RestStickerPack.Construct,
            IStickerPack.FetchManyRoute(IPathable.Empty)
        );
        Stickers = new(this, id => new RestStickerActor(this, StickerIdentity.Of(id)));
        Invites = new(this, id => new RestInviteActor(this, InviteIdentity.Of(id)));
    }

    public ValueTask DisposeAsync()
    {
        // TODO release managed resources here

        return ValueTask.CompletedTask;
    }
}