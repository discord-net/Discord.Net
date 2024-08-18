using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;
using System.Globalization;

namespace Discord.Rest;

using BansPager =
    RestPagedIndexableLink<RestBanActor, ulong, RestBan, IBanModel, IEnumerable<IBanModel>, PageGuildBansParams>;
using MembersPager =
    RestPagedIndexableLink<RestMemberActor, ulong, RestMember, IMemberModel, IEnumerable<IMemberModel>,
        PageGuildMembersParams>;
using EnumerableGuildChannelActor =
    RestEnumerableIndexableLink<RestGuildChannelActor, ulong, RestGuildChannel, IGuildChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableIntegrationActor =
    RestEnumerableIndexableLink<RestIntegrationActor, ulong, RestIntegration, IIntegration,
        IEnumerable<IIntegrationModel>>;
using EnumerableStageChannelActor =
    RestEnumerableIndexableLink<RestStageChannelActor, ulong, RestStageChannel, IStageChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableMediaChannelActor =
    RestEnumerableIndexableLink<RestMediaChannelActor, ulong, RestMediaChannel, IMediaChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableTextChannelActor =
    RestEnumerableIndexableLink<RestTextChannelActor, ulong, RestTextChannel, ITextChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableVoiceChannelActor =
    RestEnumerableIndexableLink<RestVoiceChannelActor, ulong, RestVoiceChannel, IVoiceChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableCategoryChannelActor =
    RestEnumerableIndexableLink<RestCategoryChannelActor, ulong, RestCategoryChannel, ICategoryChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableAnnouncementChannelActor =
    RestEnumerableIndexableLink<RestNewsChannelActor, ulong, RestNewsChannel, INewsChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableIntegrationChannelActor = RestEnumerableIndexableLink<
    RestIntegrationChannelTrait,
    ulong,
    RestGuildChannel,
    IIntegrationChannel,
    IEnumerable<IGuildChannelModel>
>;
using EnumerableThreadChannelActor =
    RestEnumerableIndexableLink<RestThreadChannelActor, ulong, RestThreadChannel, IThreadChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableForumChannelActor =
    RestEnumerableIndexableLink<RestForumChannelActor, ulong, RestForumChannel, IForumChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableGuildEmotesActor =
    RestEnumerableIndexableLink<RestGuildEmoteActor, ulong, RestGuildEmote, IGuildEmote,
        IEnumerable<IGuildEmoteModel>>;
using EnumerableRolesActor =
    RestEnumerableIndexableLink<RestRoleActor, ulong, RestRole, IRole, IEnumerable<IRoleModel>>;
using EnumerableGuildStickersActor =
    RestEnumerableIndexableLink<RestGuildStickerActor, ulong, RestGuildSticker, IGuildSticker,
        IEnumerable<IGuildStickerModel>>;
using EnumerableGuildScheduledEventActor =
    RestEnumerableIndexableLink<RestGuildScheduledEventActor, ulong, RestGuildScheduledEvent, IGuildScheduledEvent,
        IEnumerable<IGuildScheduledEventModel>>;
using EnumerableInviteActor =
    RestEnumerableIndexableLink<RestGuildInviteActor, string, RestGuildInvite, IGuildInvite,
        IEnumerable<IInviteModel>>;
using EnumerableWebhookActor =
    RestEnumerableIndexableLink<RestWebhookActor, ulong, RestWebhook, IWebhook, IEnumerable<IWebhookModel>>;
using ManagedRolesActor =
    RestManagedEnumerableLink<RestRoleActor, ulong, RestRole, IRole, IRoleModel>;
using ManagedEmotesActor =
    RestManagedEnumerableLink<RestGuildEmoteActor, ulong, RestGuildEmote, IGuildEmote, IGuildEmoteModel>;
using ManagedStickersActor =
    RestManagedEnumerableLink<RestGuildStickerActor, ulong, RestGuildSticker, IGuildSticker, IGuildStickerModel>;

[ExtendInterfaceDefaults]
public sealed partial class RestGuildActor :
    RestActor<ulong, RestGuild, GuildIdentity>,
    IGuildActor
{
    [SourceOfTruth] public EnumerableGuildChannelActor Channels { get; }

    [SourceOfTruth] public EnumerableTextChannelActor TextChannels { get; }

    [SourceOfTruth] public EnumerableVoiceChannelActor VoiceChannels { get; }

    [SourceOfTruth] public EnumerableCategoryChannelActor CategoryChannels { get; }

    [SourceOfTruth] public EnumerableAnnouncementChannelActor AnnouncementChannels { get; }

    [SourceOfTruth] public EnumerableThreadChannelActor ThreadChannels { get; }

    [SourceOfTruth] public EnumerableStageChannelActor StageChannels { get; }

    [SourceOfTruth] public EnumerableThreadChannelActor ActiveThreadChannels { get; }

    [SourceOfTruth] public EnumerableForumChannelActor ForumChannels { get; }

    [SourceOfTruth] public EnumerableMediaChannelActor MediaChannels { get; }

    [SourceOfTruth] public EnumerableIntegrationChannelActor IntegrationChannels { get; }

    [SourceOfTruth] public EnumerableIntegrationActor Integrations { get; }

    [SourceOfTruth] public BansPager Bans { get; }

    [SourceOfTruth] public MembersPager Members { get; }

    [SourceOfTruth] public EnumerableGuildEmotesActor Emotes { get; }

    [SourceOfTruth] public EnumerableRolesActor Roles { get; }

    [SourceOfTruth] public EnumerableGuildStickersActor Stickers { get; }

    [SourceOfTruth] public EnumerableGuildScheduledEventActor ScheduledEvents { get; }

    [SourceOfTruth] public EnumerableInviteActor Invites { get; }

    [SourceOfTruth] public EnumerableWebhookActor Webhooks { get; }

    [SourceOfTruth] public RestCurrentMemberActor CurrentMember { get; }

    internal override GuildIdentity Identity { get; }

    [TypeFactory]
    public RestGuildActor(
        DiscordRestClient client,
        GuildIdentity guild,
        CurrentMemberIdentity? currentMember = null
    ) : base(client, guild)
    {
        Identity = guild | this;

        CurrentMember = currentMember?.Actor ?? new(
            client,
            Identity,
            currentMember ?? CurrentMemberIdentity.Of(client.CurrentUser.Id)
        );

        MediaChannels = RestActors.GuildRelatedEntity(Template.T<RestMediaChannelActor>(), client, this);
        Channels = RestActors.GuildRelatedEntity(Template.T<RestGuildChannelActor>(), client, this);
        TextChannels = RestActors.GuildRelatedEntity(Template.T<RestTextChannelActor>(), client, this);
        VoiceChannels = RestActors.GuildRelatedEntity(Template.T<RestVoiceChannelActor>(), client, this);
        CategoryChannels = RestActors.GuildRelatedEntity(Template.T<RestCategoryChannelActor>(), client, this);
        AnnouncementChannels = RestActors.GuildRelatedEntity(Template.T<RestNewsChannelActor>(), client, this);
        ThreadChannels = RestActors.GuildRelatedEntity(Template.T<RestThreadChannelActor>(), client, this);
        StageChannels = RestActors.GuildRelatedEntity(Template.T<RestStageChannelActor>(), client, this);
        ForumChannels = RestActors.GuildRelatedEntity(Template.T<RestForumChannelActor>(), client, this);
        ActiveThreadChannels =
            RestActors.GuildRelatedEntityWithTransform(
                Template.T<RestThreadChannelActor>(),
                client,
                this,
                Routes.ListActiveGuildThreads(Identity.Id),
                api => api.Threads
            );
        IntegrationChannels =
            RestActors.GuildRelatedTrait(
                Template.T<RestIntegrationChannelTrait>(),
                Template.T<RestGuildChannelActor>(),
                client,
                this,
                IGuildChannel.FetchManyRoute(this),
                model => IIntegrationChannelTrait.ImplementsTraitByModel(model.GetType())
            );

        Integrations = RestActors.GuildRelatedEntity(Template.T<RestIntegrationActor>(), client, this);
        Bans = RestActors.Bans(client, this);
        Members = RestActors.Members(client, this);

        if (Identity.Detail is IdentityDetail.Entity && Identity.Entity is not null)
        {
            Roles = Identity.Entity.Roles;
            Stickers = Identity.Entity.Stickers;
            Emotes = Identity.Entity.Emotes;
        }
        else
        {
            Emotes = RestActors.GuildRelatedEntity(Template.T<RestGuildEmoteActor>(), client, this);
            Roles = RestActors.GuildRelatedEntity(Template.T<RestRoleActor>(), client, this);
            Stickers = RestActors.GuildRelatedEntity(Template.T<RestGuildStickerActor>(), client, this);
        }

        ScheduledEvents = RestActors.GuildRelatedEntity(Template.T<RestGuildScheduledEventActor>(), client, this);

        Invites = RestActors.Fetchable(
            Template.T<RestGuildInviteActor>(),
            Client,
            RestGuildInviteActor.Factory,
            Identity,
            entityFactory: RestGuildInvite.Construct,
            new RestGuildInvite.Context(Identity),
            IGuildInvite.FetchManyRoute(this)
        );

        Webhooks = RestActors.Fetchable(
            Template.T<RestWebhookActor>(),
            client,
            RestWebhookActor.Factory,
            RestWebhook.Construct,
            IWebhook.GetGuildWebhooksRoute(this)
        );
    }

    [SourceOfTruth]
    internal RestGuild CreateEntity(IGuildModel model)
        => RestGuild.Construct(Client, model);

    [SourceOfTruth]
    internal RestGuildChannel CreateEntity(IGuildChannelModel model)
        => RestGuildChannel.Construct(Client, Identity, model);

    [SourceOfTruth]
    internal RestMember CreateEntity(IMemberModel model)
        => RestMember.Construct(Client, Identity, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestGuild :
    RestPartialGuild,
    IGuild,
    IConstructable<RestGuild, IGuildModel, DiscordRestClient>
{
    [SourceOfTruth] public RestVoiceChannelActor? AFKChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? WidgetChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? SafetyAlertsChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? SystemChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? RulesChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? PublicUpdatesChannel { get; private set; }

    [SourceOfTruth] public RestMemberActor Owner { get; private set; }

    [SourceOfTruth] public ManagedRolesActor Roles { get; }
    [SourceOfTruth] public ManagedEmotesActor Emotes { get; }
    [SourceOfTruth] public ManagedStickersActor Stickers { get; }

    public int AFKTimeout => Model.AFKTimeout;

    public bool IsWidgetEnabled => Model.WidgetEnabled;

    public DefaultMessageNotifications DefaultMessageNotifications =>
        (DefaultMessageNotifications)Model.DefaultMessageNotifications;

    public MfaLevel MfaLevel => (MfaLevel)Model.MFALevel;

    public new GuildFeatures Features => base.Features ?? default;

    public PremiumTier PremiumTier => (PremiumTier)Model.PremiumTier;

    public SystemChannelFlags SystemChannelFlags => (SystemChannelFlags)Model.SystemChannelFlags;

    public new NsfwLevel NsfwLevel => base.NsfwLevel ?? NsfwLevel.Default;

    public bool IsBoostProgressBarEnabled => Model.PremiumProgressBarEnabled;

    public new int PremiumSubscriptionCount => base.PremiumSubscriptionCount ?? 0;

    public int? MaxPresences => Model.MaxPresence;

    public int? MaxMembers => Model.MaxMembers;

    public int? MaxVideoChannelUsers => Model.MaxVideoChannelUsers;

    public int? MaxStageVideoChannelUsers => Model.MaxStageVideoChannelUsers;

    public int? ApproximateMemberCount => Model.ApproximateMemberCount;

    public int? ApproximatePresenceCount => Model.ApproximatePresenceCount;

    public string PreferredLocale => Model.PreferredLocale;

    public new VerificationLevel VerificationLevel => base.VerificationLevel ?? VerificationLevel.None;

    public ExplicitContentFilterLevel ExplicitContentFilter => (ExplicitContentFilterLevel)Model.ExplicitContentFilter;

    public string? DiscoverySplashId => Model.DiscoverySplashId;

    public ulong? ApplicationId => Model.ApplicationId;

    internal override IGuildModel Model => _model;

    [ProxyInterface] internal RestGuildActor Actor { get; }

    private IGuildModel _model;

    internal RestGuild(
        DiscordRestClient client,
        IGuildModel model,
        RestGuildActor? actor = null
    ) : base(client, model)
    {
        _model = model;

        if (
            model is not (
            IModelSourceOfMultiple<IRoleModel> roles and
            IModelSourceOfMultiple<IGuildStickerModel> stickers and
            IModelSourceOfMultiple<IGuildEmoteModel> emotes
            )
        ) throw new ArgumentException($"{nameof(model)} must provide a collection of roles, stickers, and emotes.");

        var identity = GuildIdentity.Of(this);

        Emotes = RestManagedEnumerableActor.Create(
            Template.T<RestGuildEmoteActor>(),
            client,
            emotes.GetModels(),
            RestGuildEmoteActor.Factory,
            identity,
            entityFactory: RestGuildEmote.Construct,
            identity,
            IGuildEmote.FetchManyRoute(this)
        );

        Stickers = RestManagedEnumerableActor.Create(
            Template.T<RestGuildStickerActor>(),
            client,
            stickers.GetModels(),
            RestGuildStickerActor.Factory,
            identity,
            entityFactory: RestGuildSticker.Construct,
            identity,
            IGuildSticker.FetchManyRoute(this)
        );

        Roles = RestManagedEnumerableActor.Create(
            Template.T<RestRoleActor>(),
            client,
            roles.GetModels(),
            RestRoleActor.Factory,
            identity,
            entityFactory: RestRole.Construct,
            identity,
            IRole.FetchManyRoute(this)
        );

        Actor = actor ?? new(client, identity);

        AFKChannel = model.AFKChannelId
            .Map(
                static (id, client, identity) => new RestVoiceChannelActor(
                    client,
                    identity,
                    VoiceChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        WidgetChannel = model.WidgetChannelId
            .Map(
                static (id, client, identity) => new RestTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        SafetyAlertsChannel = model.SafetyAlertsChannelId
            .Map(
                static (id, client, identity) => new RestTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        SystemChannel = model.SystemChannelId
            .Map(
                static (id, client, identity) => new RestTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        RulesChannel = model.RulesChannelId
            .Map(
                static (id, client, identity) => new RestTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        PublicUpdatesChannel = model.PublicUpdatesChannelId
            .Map(
                static (id, client, identity) => new RestTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        Owner = new(client, Actor.Identity, MemberIdentity.Of(model.OwnerId));
    }

    public static RestGuild Construct(DiscordRestClient client, IGuildModel model)
        => new(client, model);

    public ValueTask UpdateAsync(IGuildModel model, CancellationToken token = default)
    {
        if (model is IModelSourceOfMultiple<IRoleModel> roles)
            Roles.Update(roles.GetModels());

        AFKChannel = AFKChannel.UpdateFrom(
            model.AFKChannelId,
            RestVoiceChannelActor.Factory,
            Client,
            Actor.Identity
        );

        WidgetChannel = WidgetChannel.UpdateFrom(
            model.WidgetChannelId,
            RestTextChannelActor.Factory,
            Client,
            Actor.Identity
        );

        SafetyAlertsChannel = SafetyAlertsChannel.UpdateFrom(
            model.SafetyAlertsChannelId,
            RestTextChannelActor.Factory,
            Client,
            Actor.Identity
        );

        SystemChannel = SystemChannel.UpdateFrom(
            model.SystemChannelId,
            RestTextChannelActor.Factory,
            Client,
            Actor.Identity
        );

        RulesChannel = RulesChannel.UpdateFrom(
            model.RulesChannelId,
            RestTextChannelActor.Factory,
            Client,
            Actor.Identity
        );

        PublicUpdatesChannel = PublicUpdatesChannel.UpdateFrom(
            model.PublicUpdatesChannelId,
            RestTextChannelActor.Factory,
            Client,
            Actor.Identity
        );

        Owner = Owner.UpdateFrom(
            model.OwnerId,
            RestMemberActor.Factory,
            Client,
            Actor.Identity
        );

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override IGuildModel GetModel() => Model;
}
