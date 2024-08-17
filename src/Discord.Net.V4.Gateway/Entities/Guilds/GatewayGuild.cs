using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Discord.Gateway.GatewayActors;
using static Discord.Template;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
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

    [SourceOfTruth] public GatewayIntegrationChannels IntegrationChannels { get; }

    [SourceOfTruth] public GatewayIntegrations Integrations { get; }

    [SourceOfTruth] public BansPager Bans { get; }

    [SourceOfTruth] public MembersPager Members { get; }

    [SourceOfTruth] public GatewayGuildEmotes Emotes { get; }

    [SourceOfTruth] public GatewayRoles Roles { get; }

    [SourceOfTruth] public GatewayGuildStickers Stickers { get; }

    [SourceOfTruth] public GatewayGuildScheduledEvents ScheduledEvents { get; }

    [SourceOfTruth] public GatewayGuildInvites Invites { get; }

    [SourceOfTruth] public Webhooks Webhooks { get; }

    internal override GuildIdentity Identity { get; }

    public GatewayGuildActor(
        DiscordGatewayClient client,
        GuildIdentity guild
    ) : base(client, guild)
    {
        Identity = guild | this;

        CurrentMember = new GatewayCurrentMemberActor(
            client,
            Identity,
            CurrentMemberIdentity.Of(Client.CurrentUser.Id)
        );
        
        Webhooks = new(
            client,
            client.Webhooks,
            model => RestWebhook.Construct(client.Rest, model),
            CachePath,
            IWebhook.GetGuildWebhooksRoute(this)
        );
        
        Invites = GuildInvites(client, Identity, CachePath);

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
                RestGuildIdentity.Of(guild.Id)
            )
        );
        IntegrationChannels = GuildRelatedTrait<RestGuildChannel>(
            T<GatewayIntegrationChannelTrait>(),
            T<GatewayGuildChannelActor>(),
            client,
            Identity,
            CachePath,
            model => IIntegrationChannelTrait.ImplementsTraitByModel(model.GetType())
        );

        ActiveThreadChannels = GuildRelatedEntity<RestThreadChannel>(
            T<GatewayThreadChannelActor>(),
            client,
            Identity,
            CachePath,
            static guild => new RestThreadChannel.Context(
                RestGuildIdentity.Of(guild.Id)
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
    internal GatewayGuildChannel CreateEntity(IGuildChannelModel model)
        => Client.StateController.CreateLatent(Channels[model.Id], model, CachePath);

    [SourceOfTruth]
    internal GatewayMember CreateEntity(IMemberModel model)
        => Client.StateController.CreateLatent(Members[model.Id], model, CachePath);
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

    public GuildFeatures Features { get; private set; } = GuildFeatures.Empty;

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

        UpdateLinkedActors(model);
        UpdateComputed(model);

        Owner ??= Owner.UpdateFrom(
            model.OwnerId,
            static (guild, member) => guild.Members[member],
            this
        );
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

    private void UpdateLinkedActors(IGuildModel model)
    {
        AFKChannel = AFKChannel.UpdateFrom(
            model.AFKChannelId,
            static (guild, channel) => guild.VoiceChannels[channel],
            this
        );

        WidgetChannel = WidgetChannel.UpdateFrom(
            model.WidgetChannelId,
            static (guild, channel) => guild.TextChannels[channel],
            this
        );

        SafetyAlertsChannel = SafetyAlertsChannel.UpdateFrom(
            model.SafetyAlertsChannelId,
            static (guild, channel) => guild.TextChannels[channel],
            this
        );

        SystemChannel = SystemChannel.UpdateFrom(
            model.SafetyAlertsChannelId,
            static (guild, channel) => guild.TextChannels[channel],
            this
        );

        RulesChannel = RulesChannel.UpdateFrom(
            model.SafetyAlertsChannelId,
            static (guild, channel) => guild.TextChannels[channel],
            this
        );

        PublicUpdatesChannel = PublicUpdatesChannel.UpdateFrom(
            model.SafetyAlertsChannelId,
            static (guild, channel) => guild.TextChannels[channel],
            this
        );

        Owner = Owner.UpdateFrom(
            model.OwnerId,
            static (guild, member) => guild.Members[member],
            this
        );
    }

    private void UpdateComputed(IGuildModel model)
    {
        if(!Features.Equals(model.Features))
            Features = new GuildFeatures(model.Features);
    }

    public override ValueTask UpdateAsync(IGuildModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        UpdateLinkedActors(model);
        UpdateComputed(model);

        Model = model;

        return ValueTask.CompletedTask;
    }

    [SourceOfTruth]
    public override IGuildModel GetModel() => Model;
}
