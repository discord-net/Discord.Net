using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Globalization;
using static Discord.Gateway.GatewayActors;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayGuildActor :
    GatewayCachedActor<ulong, GatewayGuild, GuildIdentity, IGuildModel>,
    IGuildActor
{
    [SourceOfTruth] public GatewayCurrentMemberActor CurrentMember { get; }
    [SourceOfTruth] public GatewayGuildChannels Channels { get; }

    [SourceOfTruth] public GatewayTextChannels TextChannels { get; }

    [SourceOfTruth] public GatewayVoiceChannels VoiceChannels { get; }

    [SourceOfTruth] public GatewayCategoryChannels CategoryChannels { get; }

    [SourceOfTruth] public GatewayNewsChannels AnnouncementChannels { get; }

    [SourceOfTruth] public GatewayThreadChannels ThreadChannels { get; }

    [SourceOfTruth] public GatewayActiveThreads ActiveThreadChannels { get; }

    [SourceOfTruth] public GatewayStageChannels StageChannels { get; }

    [SourceOfTruth] public GatewayForumChannels ForumChannels { get; }

    [SourceOfTruth] public GatewayMediaChannels MediaChannels { get; }

    [SourceOfTruth] public GatewayIntegrations Integrations { get; }

    [SourceOfTruth] public BansPager Bans { get; }

    [SourceOfTruth] public MembersPager Members { get; }

    [SourceOfTruth] public GatewayGuildEmotes Emotes { get; }

    [SourceOfTruth] public GatewayRoles Roles { get; }

    [SourceOfTruth] public GatewayGuildStickers Stickers { get; }

    [SourceOfTruth] public GatewayGuildScheduledEvents ScheduledEvents { get; }

    [SourceOfTruth] public GatewayGuildInvites Invites { get; }

    [SourceOfTruth] public IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks { get; }

    internal override GuildIdentity Identity { get; }

    public GatewayGuildActor(DiscordGatewayClient client, GuildIdentity guild)
        : base(client, guild)
    {
        Identity = guild | this;

        CurrentMember = new GatewayCurrentMemberActor(
            client,
            Identity,
            CurrentMemberIdentity.Of(Client.CurrentUser.Id)
        );

        Channels =
            GuildRelatedEntity<RestGuildChannel>(Of<GatewayGuildChannelActor>(), client, Identity, CachePath);
        TextChannels =
            GuildRelatedEntity<RestTextChannel>(Of<GatewayTextChannelActor>(), client, Identity, CachePath);
        VoiceChannels =
            GuildRelatedEntity<RestVoiceChannel>(T<GatewayVoiceChannelActor>(), client, Identity, CachePath);
        CategoryChannels =
            GuildRelatedEntity<RestCategoryChannel>(T<GatewayCategoryChannelActor>(), client, Identity, CachePath);
        AnnouncementChannels =
            GuildRelatedEntity<RestNewsChannel>(T<GatewayNewsChannelActor>(), client, Identity, CachePath);
        StageChannels =
            GuildRelatedEntity<RestStageChannel>(T<GatewayStageChannelActor>(), client, Identity, CachePath);
        ForumChannels =
            GuildRelatedEntity<RestForumChannel>(T<GatewayForumChannelActor>(), client, Identity, CachePath);
        MediaChannels =
            GuildRelatedEntity<RestMediaChannel>(T<GatewayMediaChannelActor>(), client, Identity, CachePath);
        ThreadChannels = GuildRelatedEntity<RestThreadChannel>(
            T<GatewayThreadChannelActor>(),
            client,
            Identity,
            CachePath,
            static guild => new RestThreadChannel.Context(
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id)
            )
        );

        ActiveThreadChannels = GuildRelatedEntity<RestThreadChannel>(
            T<GatewayThreadChannelActor>(),
            client,
            Identity,
            CachePath,
            static guild => new RestThreadChannel.Context(
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id)
            ),
            Routes.ListActiveGuildThreads(Identity.Id),
            api => api.Threads
        );

        Integrations =
            GuildRelatedEntity<RestIntegration>(T<GatewayIntegrationActor>(), client, Identity, CachePath);
        ScheduledEvents = GuildRelatedEntity<RestGuildScheduledEvent>(
            T<GatewayGuildScheduledEventActor>(),
            client,
            Identity,
            CachePath
        );

        Stickers = GuildRelatedEntity<RestGuildSticker>(T<GatewayGuildStickerActor>(), client, Identity, CachePath);
        Emotes = GuildRelatedEntity<RestGuildEmote>(T<GatewayGuildEmoteActor>(), client, Identity, CachePath);
        Roles = GuildRelatedEntity<RestRole>(T<GatewayRoleActor>(), client, Identity, CachePath);

        Bans = PageBans(client, guild, CachePath);
        Members = PageMembers(client, guild, CachePath);
    }

    [SourceOfTruth]
    internal GatewayGuild CreateEntity(IGuildModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);

    [SourceOfTruth]
    internal IGuildChannel CreateEntity(IGuildChannelModel model) => throw new NotImplementedException();

    [SourceOfTruth]
    internal IMember CreateEntity(IMemberModel model) => throw new NotImplementedException();
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
        );
    }

    [SourceOfTruth]
    public override IGuildModel GetModel() => Model;

    public override ValueTask UpdateAsync(IGuildModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public static GatewayGuild Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildModel model)
    {
        return new GatewayGuild(
            client,
            model,
            context.TryGetActor<GatewayGuildActor>()
        );
    }

    //IPartialGuildModel IEntityOf<IPartialGuildModel>.GetModel() => GetModel();
}
