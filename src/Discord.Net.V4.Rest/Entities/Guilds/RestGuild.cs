using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds.Integrations;
using Discord.Rest.Invites;
using Discord.Rest.Stickers;
using Discord.Rest.Webhooks;
using System.Globalization;

namespace Discord.Rest.Guilds;

using BansPager = RestPagedIndexableActor<RestBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams>;
using MembersPager = RestPagedIndexableActor<RestGuildMemberActor, ulong, RestGuildMember, IEnumerable<IMemberModel>,
    PageGuildMembersParams>;
using EnumerableGuildChannelActor =
    RestEnumerableIndexableActor<RestGuildChannelActor, ulong, RestGuildChannel, IGuildChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableIntegrationActor =
    RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IIntegration,
        IEnumerable<IIntegrationModel>>;
using EnumerableStageChannelActor =
    RestEnumerableIndexableActor<RestStageChannelActor, ulong, RestStageChannel, IStageChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableMediaChannelActor =
    RestEnumerableIndexableActor<RestMediaChannelActor, ulong, RestMediaChannel, IMediaChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableTextChannelActor =
    RestEnumerableIndexableActor<RestTextChannelActor, ulong, RestTextChannel, ITextChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableVoiceChannelActor =
    RestEnumerableIndexableActor<RestVoiceChannelActor, ulong, RestVoiceChannel, IVoiceChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableCategoryChannelActor =
    RestEnumerableIndexableActor<RestCategoryChannelActor, ulong, RestCategoryChannel, ICategoryChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableAnnouncementChannelActor =
    RestEnumerableIndexableActor<RestNewsChannelActor, ulong, RestNewsChannel, INewsChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableThreadChannelActor =
    RestEnumerableIndexableActor<RestThreadChannelActor, ulong, RestThreadChannel, IThreadChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableForumChannelActor =
    RestEnumerableIndexableActor<RestForumChannelActor, ulong, RestForumChannel, IForumChannel,
        IEnumerable<IGuildChannelModel>>;
using EnumerableGuildEmotesActor =
    RestEnumerableIndexableActor<RestGuildEmoteActor, ulong, RestGuildEmote, IGuildEmote,
        IEnumerable<IGuildEmoteModel>>;
using EnumerableRolesActor =
    RestEnumerableIndexableActor<RestRoleActor, ulong, RestRole, IRole, IEnumerable<IRoleModel>>;
using EnumerableGuildStickersActor =
    RestEnumerableIndexableActor<RestGuildStickerActor, ulong, RestGuildSticker, IGuildSticker,
        IEnumerable<IGuildStickerModel>>;
using EnumerableGuildScheduledEventActor =
    RestEnumerableIndexableActor<RestGuildScheduledEventActor, ulong, RestGuildScheduledEvent, IGuildScheduledEvent,
        IEnumerable<IGuildScheduledEventModel>>;
using EnumerableInviteActor =
    RestEnumerableIndexableActor<RestInviteActor, string, RestInvite, IInvite, IEnumerable<IInviteModel>>;
using EnumerableWebhookActor =
    RestEnumerableIndexableActor<RestWebhookActor, ulong, RestWebhook, IWebhook, IEnumerable<IWebhookModel>>;
using ManagedRolesActor =
    RestManagedEnumerableActor<RestRoleActor, ulong, RestRole, IRole, IRoleModel>;
using ManagedEmotesActor =
    RestManagedEnumerableActor<RestGuildEmoteActor, ulong, RestGuildEmote, IGuildEmote, IGuildEmoteModel>;
using ManagedStickersActor =
    RestManagedEnumerableActor<RestGuildStickerActor, ulong, RestGuildSticker, IGuildSticker, IGuildStickerModel>;

[ExtendInterfaceDefaults(typeof(IGuildActor))]
public sealed partial class RestGuildActor :
    RestActor<ulong, RestGuild, GuildIdentity>,
    IGuildActor
{
    public override GuildIdentity Identity { get; }

    [TypeFactory]
    public RestGuildActor(
        DiscordRestClient client,
        GuildIdentity guild
    ) : base(client, guild)
    {
        guild = Identity = guild.MostSpecific(this);

        MediaChannels = RestActors.GuildRelatedEntity(Template.Of<RestMediaChannelActor>(), client, this);
        Channels = RestActors.GuildRelatedEntity(Template.Of<RestGuildChannelActor>(), client, this);
        TextChannels = RestActors.GuildRelatedEntity(Template.Of<RestTextChannelActor>(), client, this);
        VoiceChannels = RestActors.GuildRelatedEntity(Template.Of<RestVoiceChannelActor>(), client, this);
        CategoryChannels = RestActors.GuildRelatedEntity(Template.Of<RestCategoryChannelActor>(), client, this);
        AnnouncementChannels = RestActors.GuildRelatedEntity(Template.Of<RestNewsChannelActor>(), client, this);
        ThreadChannels = RestActors.GuildRelatedEntity(Template.Of<RestThreadChannelActor>(), client, this);
        StageChannels = RestActors.GuildRelatedEntity(Template.Of<RestStageChannelActor>(), client, this);
        ForumChannels = RestActors.GuildRelatedEntity(Template.Of<RestForumChannelActor>(), client, this);
        ActiveThreadChannels =
            RestActors.GuildRelatedEntityWithTransform(
                Template.Of<RestThreadChannelActor>(),
                client,
                this,
                Routes.ListActiveGuildThreads(guild.Id),
                api => api.Threads
            );
        Integrations = RestActors.GuildRelatedEntity(Template.Of<RestIntegrationActor>(), client, this);
        Bans = RestActors.Bans(client, guild);
        Members = RestActors.Members(client, guild);

        if (guild.Detail is IdentityDetail.Entity && guild.Entity is not null)
        {
            Roles = guild.Entity.Roles;
            Stickers = guild.Entity.Stickers;
            Emotes = guild.Entity.Emotes;
        }
        else
        {
            Emotes = RestActors.GuildRelatedEntity(Template.Of<RestGuildEmoteActor>(), client, this);
            Roles = RestActors.GuildRelatedEntity(Template.Of<RestRoleActor>(), client, this);
            Stickers = RestActors.GuildRelatedEntity(Template.Of<RestGuildStickerActor>(), client, this);
        }

        ScheduledEvents = RestActors.GuildRelatedEntity(Template.Of<RestGuildScheduledEventActor>(), client, this);

        Invites = RestActors.Fetchable(
            Template.Of<RestInviteActor>(),
            Client,
            RestInviteActor.Factory,
            guild,
            entityFactory: RestInvite.Construct,
            new RestInvite.Context(guild),
            IInvite.GetGuildInvitesRoute(this)
        );

        Webhooks = RestActors.Fetchable(
            Template.Of<RestWebhookActor>(),
            client,
            RestWebhookActor.Factory,
            RestWebhook.Construct,
            IWebhook.GetGuildWebhooksRoute(this)
        );
    }

    [SourceOfTruth] public EnumerableMediaChannelActor MediaChannels { get; }

    [SourceOfTruth] public EnumerableGuildChannelActor Channels { get; }

    [SourceOfTruth] public EnumerableTextChannelActor TextChannels { get; }

    [SourceOfTruth] public EnumerableVoiceChannelActor VoiceChannels { get; }

    [SourceOfTruth] public EnumerableCategoryChannelActor CategoryChannels { get; }

    [SourceOfTruth] public EnumerableAnnouncementChannelActor AnnouncementChannels { get; }

    [SourceOfTruth] public EnumerableThreadChannelActor ThreadChannels { get; }

    [SourceOfTruth] public EnumerableStageChannelActor StageChannels { get; }

    [SourceOfTruth] public EnumerableThreadChannelActor ActiveThreadChannels { get; }

    [SourceOfTruth] public EnumerableIntegrationActor Integrations { get; }

    [SourceOfTruth] public BansPager Bans { get; }

    [SourceOfTruth] public EnumerableForumChannelActor ForumChannels { get; }

    [SourceOfTruth] public MembersPager Members { get; }

    [SourceOfTruth] public EnumerableGuildEmotesActor Emotes { get; }

    [SourceOfTruth] public EnumerableRolesActor Roles { get; }

    [SourceOfTruth] public EnumerableGuildStickersActor Stickers { get; }

    [SourceOfTruth] public EnumerableGuildScheduledEventActor ScheduledEvents { get; }

    [SourceOfTruth] public EnumerableInviteActor Invites { get; }

    [SourceOfTruth] public EnumerableWebhookActor Webhooks { get; }

    [SourceOfTruth]
    internal RestGuild CreateEntity(IGuildModel model)
        => RestGuild.Construct(Client, model);

    [SourceOfTruth]
    internal RestGuildChannel CreateEntity(IGuildChannelModel model)
        => RestGuildChannel.Construct(Client, Identity, model);

    [SourceOfTruth]
    internal RestGuildMember CreateEntity(IMemberModel model)
        => RestGuildMember.Construct(Client, Identity, model);
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

    [SourceOfTruth] public RestGuildMemberActor Owner { get; private set; }

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

    public CultureInfo PreferredCulture => CultureInfo.GetCultureInfoByIetfLanguageTag(Model.PreferredLocale);

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

    public string? DiscoverySplashId => Model.DiscoverySplash;

    public ulong? ApplicationId => Model.ApplicationId;

    internal override IGuildModel Model => _model;

    [ProxyInterface(
        typeof(IGuildActor),
        typeof(IEntityProvider<IGuild, IGuildModel>)
    )]
    internal RestGuildActor Actor { get; }

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
            Template.Of<RestGuildEmoteActor>(),
            client,
            emotes.GetModels(),
            RestGuildEmoteActor.Factory,
            identity,
            entityFactory: RestGuildEmote.Construct,
            identity,
            IGuildEmote.FetchManyRoute(this)
        );

        Stickers = RestManagedEnumerableActor.Create(
            Template.Of<RestGuildStickerActor>(),
            client,
            stickers.GetModels(),
            RestGuildStickerActor.Factory,
            identity,
            entityFactory: RestGuildSticker.Construct,
            identity,
            IGuildSticker.FetchManyRoute(this)
        );

        Roles = RestManagedEnumerableActor.Create(
            Template.Of<RestRoleActor>(),
            client,
            roles.GetModels(),
            RestRoleActor.Factory,
            identity,
            entityFactory: RestRole.Construct,
            identity,
            IRole.FetchManyRoute(this)
        );

        Actor = actor ?? new(client, identity);


        // Roles = RestManagedEnumerableActor.Create<RestRoleActor, ulong, RestRole, IRole, IRoleModel, GuildIdentity>(
        //     client,
        //     roles.GetModels(),
        //     id => new(client, Actor.Identity, RoleIdentity.Of(id)),
        //     Routes.GetGuildRoles(model.Id),
        //     Actor.Identity
        // );

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
            RestGuildMemberActor.Factory,
            Client,
            Actor.Identity
        );

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override IGuildModel GetModel() => Model;
}
