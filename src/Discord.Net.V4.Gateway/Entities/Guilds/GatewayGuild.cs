using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Globalization;
using static Discord.Gateway.GatewayActors;
using static Discord.Template;

namespace Discord.Gateway;

using GatewayGuildChannels = GatewayEnumerableIndexableActor<
    GatewayGuildChannelActor,
    ulong,
    GatewayGuildChannel,
    RestGuildChannel,
    IGuildChannel,
    IGuildChannelModel
>;
using GatewayTextChannels = GatewayEnumerableIndexableActor<
    GatewayTextChannelActor,
    ulong,
    GatewayTextChannel,
    RestTextChannel,
    ITextChannel,
    IGuildTextChannelModel,
    IGuildChannelModel
>;
using GatewayVoiceChannels = GatewayEnumerableIndexableActor<
    GatewayVoiceChannelActor,
    ulong,
    GatewayVoiceChannel,
    RestVoiceChannel,
    IVoiceChannel,
    IGuildVoiceChannelModel,
    IGuildChannelModel
>;
using GatewayCategoryChannels = GatewayEnumerableIndexableActor<
    GatewayCategoryChannelActor,
    ulong,
    GatewayCategoryChannel,
    RestCategoryChannel,
    ICategoryChannel,
    IGuildCategoryChannelModel,
    IGuildChannelModel
>;
using GatewayNewsChannels = GatewayEnumerableIndexableActor<
    GatewayNewsChannelActor,
    ulong,
    GatewayNewsChannel,
    RestNewsChannel,
    INewsChannel,
    IGuildNewsChannelModel,
    IGuildChannelModel
>;
using GatewayThreadChannels = GatewayEnumerableIndexableActor<
    GatewayThreadChannelActor,
    ulong,
    GatewayThreadChannel,
    RestThreadChannel,
    IThreadChannel,
    IThreadChannelModel,
    IGuildChannelModel
>;
using GatewayStageChannels = GatewayEnumerableIndexableActor<
    GatewayStageChannelActor,
    ulong,
    GatewayStageChannel,
    RestStageChannel,
    IStageChannel,
    IGuildStageChannelModel,
    IGuildChannelModel
>;
using GatewayForumChannels = GatewayEnumerableIndexableActor<
    GatewayForumChannelActor,
    ulong,
    GatewayForumChannel,
    RestForumChannel,
    IForumChannel,
    IGuildForumChannelModel,
    IGuildChannelModel
>;
using GatewayMediaChannels = GatewayEnumerableIndexableActor<
    GatewayMediaChannelActor,
    ulong,
    GatewayMediaChannel,
    RestMediaChannel,
    IMediaChannel,
    IGuildMediaChannelModel,
    IGuildChannelModel
>;

using GatewayBans = GatewayPagedIndexableActor<
    GatewayBanActor,
    ulong,
    GatewayBan,

>;

public sealed partial class GatewayGuildActor :
    GatewayCachedActor<ulong, GatewayGuild, GuildIdentity, IGuildModel>,
    IGuildActor
{
    [SourceOfTruth] public GatewayGuildChannels Channels { get; }

    [SourceOfTruth] public GatewayTextChannels TextChannels { get; }

    [SourceOfTruth] public GatewayVoiceChannels VoiceChannels { get; }

    [SourceOfTruth] public GatewayCategoryChannels CategoryChannels { get; }

    [SourceOfTruth] public GatewayNewsChannels AnnouncementChannels { get; }

    [SourceOfTruth] public GatewayThreadChannels ThreadChannels { get; }

    [SourceOfTruth]
    public IEnumerableIndexableActor<IThreadChannelActor, ulong, IThreadChannel> ActiveThreadChannels { get; }

    [SourceOfTruth] public GatewayStageChannels StageChannels { get; }

    [SourceOfTruth] public GatewayForumChannels ForumChannels { get; }

    [SourceOfTruth] public GatewayMediaChannels MediaChannels { get; }

    [SourceOfTruth] public IEnumerableIndexableActor<IIntegrationActor, ulong, IIntegration> Integrations { get; }

    [SourceOfTruth] public IPagedIndexableActor<IBanActor, ulong, IBan, PageGuildBansParams> Bans { get; }

    [SourceOfTruth]
    public IPagedIndexableActor<IGuildMemberActor, ulong, IGuildMember, PageGuildMembersParams> Members =>
        throw new NotImplementedException();

    [SourceOfTruth] public IEnumerableIndexableActor<IGuildEmoteActor, ulong, IGuildEmote> Emotes { get; }

    [SourceOfTruth] public IEnumerableIndexableActor<IRoleActor, ulong, IRole> Roles { get; }

    [SourceOfTruth] public IEnumerableIndexableActor<IGuildStickerActor, ulong, IGuildSticker> Stickers { get; }

    [SourceOfTruth]
    public IEnumerableIndexableActor<IGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents { get; }

    [SourceOfTruth] public IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites { get; }

    [SourceOfTruth] public IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks { get; }

    public GatewayGuildActor(DiscordGatewayClient client, GuildIdentity guild)
        : base(client, guild)
    {
        var identity = guild | this;

        Channels = GuildRelatedEntity<RestGuildChannel>(Of<GatewayGuildChannelActor>(), client, identity, CachePath);
        TextChannels = GuildRelatedEntity<RestTextChannel>(Of<GatewayTextChannelActor>(), client, identity, CachePath);
        VoiceChannels = GuildRelatedEntity<RestVoiceChannel>(T<GatewayVoiceChannelActor>(), client, identity, CachePath);
        CategoryChannels = GuildRelatedEntity<RestCategoryChannel>(T<GatewayCategoryChannelActor>(), client, identity, CachePath);
        AnnouncementChannels = GuildRelatedEntity<RestNewsChannel>(T<GatewayNewsChannelActor>(), client, identity, CachePath);
        ThreadChannels = GuildRelatedEntity<RestThreadChannel>(
                T<GatewayThreadChannelActor>(),
                client,
                identity,
                CachePath,
                static guild => new RestThreadChannel.Context(
                    IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id)
                )
            );
        StageChannels = GuildRelatedEntity<RestStageChannel>(T<GatewayStageChannelActor>(), client, identity, CachePath);
        ForumChannels = GuildRelatedEntity<RestForumChannel>(T<GatewayForumChannelActor>(), client, identity, CachePath);
        MediaChannels = GuildRelatedEntity<RestMediaChannel>(T<GatewayMediaChannelActor>(), client, identity, CachePath);
    }

    [SourceOfTruth]
    internal GatewayGuild CreateEntity(IGuildModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);

    [SourceOfTruth]
    internal IGuildChannel CreateEntity(IGuildChannelModel model) => throw new NotImplementedException();

    [SourceOfTruth]
    internal IGuildMember CreateEntity(IMemberModel model) => throw new NotImplementedException();
}

public sealed partial class GatewayGuild :
    GatewayCacheableEntity<GatewayGuild, ulong, IGuildModel>,
    IGuild
{
    public string Name => Model.Name;

    public string? SplashId => Model.SplashId;

    public string? BannerId => Model.BannerId;

    public string? Description => Model.Description;

    public string? IconId => Model.IconId;

    public string? VanityUrlCode => Model.VanityUrlCode;

    public int AFKTimeout => Model.AFKTimeout;

    public bool IsWidgetEnabled => Model.WidgetEnabled;

    public DefaultMessageNotifications DefaultMessageNotifications =>
        (DefaultMessageNotifications)Model.DefaultMessageNotifications;

    public MfaLevel MfaLevel => (MfaLevel)Model.MFALevel;

    public GuildFeatures Features { get; private set; }

    public PremiumTier PremiumTier => (PremiumTier)Model.PremiumTier;

    public SystemChannelFlags SystemChannelFlags => (SystemChannelFlags)Model.SystemChannelFlags;

    public NsfwLevel NsfwLevel => (NsfwLevel?)Model.NsfwLevel ?? NsfwLevel.Default;

    public bool IsBoostProgressBarEnabled => Model.PremiumProgressBarEnabled;

    public int PremiumSubscriptionCount => Model.PremiumSubscriptionCount ?? 0;

    public int? MaxPresences => Model.MaxPresence;

    public int? MaxMembers => Model.MaxMembers;

    public int? MaxVideoChannelUsers => Model.MaxVideoChannelUsers;

    public int? MaxStageVideoChannelUsers => Model.MaxStageVideoChannelUsers;

    public int? ApproximateMemberCount => Model.ApproximateMemberCount;

    public int? ApproximatePresenceCount => Model.ApproximatePresenceCount;

    public string PreferredLocale => Model.PreferredLocale;

    public VerificationLevel VerificationLevel => (VerificationLevel?)Model.VerificationLevel ?? VerificationLevel.None;

    public ExplicitContentFilterLevel ExplicitContentFilter => (ExplicitContentFilterLevel)Model.ExplicitContentFilter;

    public string? DiscoverySplashId => Model.DiscoverySplashId;

    [SourceOfTruth] public GatewayVoiceChannelActor? AFKChannel { get; private set; }

    [SourceOfTruth] public GatewayTextChannelActor? WidgetChannel { get; private set; }

    [SourceOfTruth] public GatewayTextChannelActor? SafetyAlertsChannel { get; private set; }

    [SourceOfTruth] public GatewayTextChannelActor? SystemChannel { get; private set; }

    [SourceOfTruth] public GatewayTextChannelActor? RulesChannel { get; private set; }

    [SourceOfTruth] public GatewayTextChannelActor? PublicUpdatesChannel { get; private set; }

    [SourceOfTruth] public GatewayMemberActor Owner { get; private set; }

    public ulong? ApplicationId => Model.ApplicationId;

    [ProxyInterface] internal GatewayGuildActor Actor { get; }

    internal IGuildModel Model { get; private set; }

    public GatewayGuild(
        DiscordGatewayClient client,
        IGuildModel model,
        GatewayGuildActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, GuildIdentity.Of(this));
    }

    private void UpdateComputeds(IGuildModel model)
    {
        AFKChannel = model.AFKChannelId.Map(
            static (id, client, guild) => client.Guilds[guild.Id].VoiceChannels[id],
            Client,
            Actor.Identity
        )
    }

    public override IGuildModel GetModel() => Model;

    public override ValueTask UpdateAsync(IGuildModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public static GatewayGuild Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext context,
        IGuildModel model)
    {
        return new GatewayGuild(
            client,
            model,
            context.TryGetActor<GatewayGuildActor>()
        );
    }
}
