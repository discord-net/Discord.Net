using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;
using System.Globalization;
using MorseCode.ITask;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestGuildActor :
    RestActor<ulong, RestGuild, GuildIdentity, IGuildModel>,
    IGuildActor
{
    [SourceOfTruth] public ChannelsLink Channels { get; }
    [SourceOfTruth] public ThreadsLink Threads { get; }
    [SourceOfTruth] public IntegrationLink.Enumerable.Indexable.BackLink<RestGuildActor> Integrations { get; }
    [SourceOfTruth] public BanLink.Paged<PageGuildBansParams>.Indexable.BackLink<RestGuildActor> Bans { get; }
    [SourceOfTruth] public MemberLink.Paged<PageGuildMembersParams>.Indexable.BackLink<RestGuildActor> Members { get; }
    [SourceOfTruth] public GuildEmoteLink.Enumerable.Indexable.BackLink<RestGuildActor> Emotes { get; }
    [SourceOfTruth] public RoleLink.Enumerable.Indexable.BackLink<RestGuildActor> Roles { get; }
    [SourceOfTruth] public GuildStickerLink.Enumerable.Indexable.BackLink<RestGuildActor> Stickers { get; }

    [SourceOfTruth]
    public GuildScheduledEventLink.Enumerable.Indexable.BackLink<RestGuildActor> ScheduledEvents { get; }

    [SourceOfTruth] public GuildInviteLink.Enumerable.Indexable.BackLink<RestGuildActor> Invites { get; }
    [SourceOfTruth] public WebhookLink.Enumerable.Indexable.BackLink<RestGuildActor> Webhooks { get; }
    [SourceOfTruth] public RestCurrentMemberActor.BackLink<RestGuildActor> CurrentMember { get; }

    internal override GuildIdentity Identity { get; }

    [TypeFactory]
    public RestGuildActor(
        DiscordRestClient client,
        GuildIdentity guild,
        CurrentMemberIdentity? currentMember = null
    ) : base(client, guild)
    {
        Identity = guild | this;

        CurrentMember = new RestCurrentMemberActor.BackLink<RestGuildActor>(
            this,
            client,
            guild,
            currentMember ?? CurrentMemberIdentity.Of(client.CurrentUser.Id)
        );

        Channels = new(this);
        Threads = new(this);

        Integrations = RestActors.GuildRelatedLink(
            Template.Of<RestIntegrationActor>(),
            this
        );
        Bans = new RestBanLink.Paged<PageGuildBansParams, IEnumerable<IBanModel>>.Indexable.BackLink<RestGuildActor>(
            this,
            client,
            new RestActorProvider<ulong, RestBanActor>(
                (client, id) => new RestBanActor(client, Identity, BanIdentity.Of(id))
            ),
            this,
            a => a
        );
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
    internal override RestGuild CreateEntity(IGuildModel model)
        => RestGuild.Construct(Client, model);

    #region Link Types

    public sealed partial class ThreadsLink :
        RestGuildThreadChannelLink.Indexable.BackLink<RestGuildActor>,
        IGuildActor.ThreadsLink
    {
        [SourceOfTruth] public GuildThreadChannelLink.Enumerable.BackLink<RestGuildActor> Active { get; }

        public ThreadsLink(
            RestGuildActor guild
        ) : base(
            guild,
            guild.Client,
            new RestActorProvider<ulong, RestGuildThreadChannelActor>(
                (client, id) => new RestGuildThreadChannelActor(client, guild.Identity, GuildThreadIdentity.Of(id))
            )
        )
        {
            Active = new RestGuildThreadChannelLink.Enumerable.BackLink<RestGuildActor>(
                guild,
                guild.Client,
                Provider,
                (RestGuildThreadChannelLink.Enumerable.EnumerableProviderDelegate) (
                    async (client, options, token) =>
                    {
                        var model = await Routes
                            .ListActiveGuildThreads(guild.Id)
                            .AsRequiredProvider()
                            (client, options, token);

                        return model
                            .Threads
                            .Select(thread =>
                            {
                                var actor = Provider.GetActor(client, thread.Id);

                                var currentThreadMemberModel = model.Members.FirstOrDefault(x =>
                                    x.ThreadId.IsSpecified && x.ThreadId.Value == actor.Id
                                );

                                if (currentThreadMemberModel is not null)
                                    actor.CurrentThreadMember.UpdateIdentity(currentThreadMemberModel);

                                return RestThreadChannel.Construct(client, actor, thread);
                            })
                            .ToList()
                            .AsReadOnly();
                    }
                )
            );
        }

        IThreadChannel IEntityProvider<IThreadChannel, IThreadChannelModel>.CreateEntity(IThreadChannelModel model)
            => CreateEntity(model);

        IGuildThreadChannelActor IActorProvider<IDiscordClient, IGuildThreadChannelActor, ulong>.GetActor(
            IDiscordClient client, ulong id
        ) => GetActor(id);

        IGuildThreadChannelActor ILinkType<
            IGuildThreadChannelActor,
            ulong,
            IThreadChannel,
            IThreadChannelModel
        >.Indexable.Specifically(
            ulong id
        ) => Specifically(id);

        IGuildActor IBackLink<
            IGuildActor,
            IGuildThreadChannelActor,
            ulong,
            IThreadChannel,
            IThreadChannelModel
        >.Source => Source;
    }

    public sealed partial class MembersLink : 
        RestMemberLink.Paged<PageGuildMembersParams, IEnumerable<IMemberModel>>.Indexable.BackLink<RestGuildActor>,
        IGuildActor.MemebrsLink
    {
        public MembersLink(
            RestGuildActor guild 
            ) : base(guild, guild.Client, )
        {
        }

        public IGuildActor Source => throw new NotImplementedException();

        public ICurrentMemberActor Current => throw new NotImplementedException();
    }
    
    public sealed partial class ChannelsLink :
        RestGuildChannelLink.Enumerable.Indexable.BackLink<RestGuildActor>,
        IGuildActor.IGuildChannelsLink
    {
        [SourceOfTruth] public TextChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> Text { get; }

        [SourceOfTruth] public VoiceChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> Voice { get; }

        [SourceOfTruth] public CategoryChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> Category { get; }

        [SourceOfTruth] public NewsChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> News { get; }

        [SourceOfTruth] public StageChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> Stage { get; }

        [SourceOfTruth] public ForumChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> Forum { get; }

        [SourceOfTruth] public MediaChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> Media { get; }

        [SourceOfTruth]
        public IntegrationChannelTraitLink.Enumerable.Indexable.BackLink<RestGuildActor> Integration { get; }

        [SourceOfTruth] public ThreadableChannelLink.Enumerable.Indexable.BackLink<RestGuildActor> Threadable { get; }

        public ChannelsLink(RestGuildActor guild) : base(
            guild,
            guild.Client,
            new RestActorProvider<ulong, RestGuildChannelActor>(
                (client, id) => new RestGuildChannelActor(client, guild.Identity, GuildChannelIdentity.Of(id))
            ),
            Routes.GetGuildChannels(guild.Id).AsRequiredProvider()
        )
        {
            Text = RestActors.GuildChannel(Template.Of<RestTextChannelActor>(), guild, ApiProvider);
            Voice = RestActors.GuildChannel(Template.Of<RestVoiceChannelActor>(), guild, ApiProvider);
            Category = RestActors.GuildChannel(Template.Of<RestCategoryChannelActor>(), guild, ApiProvider);
            News = RestActors.GuildChannel(Template.Of<RestNewsChannelActor>(), guild, ApiProvider);
            Stage = RestActors.GuildChannel(Template.Of<RestStageChannelActor>(), guild, ApiProvider);
            Forum = RestActors.GuildChannel(Template.Of<RestForumChannelActor>(), guild, ApiProvider);
            Media = RestActors.GuildChannel(Template.Of<RestMediaChannelActor>(), guild, ApiProvider);
            Threadable = RestActors.GuildChannel(Template.Of<RestThreadableChannelActor>(), guild, ApiProvider);
            Integration = RestActors.GuildChannel(Template.Of<RestIntegrationChannelTrait>(), guild, ApiProvider);
        }

        IGuildChannel IEntityProvider<IGuildChannel, IGuildChannelModel>.CreateEntity(IGuildChannelModel model)
            => CreateEntity(model);

        IGuildChannelActor IActorProvider<IDiscordClient, IGuildChannelActor, ulong>.GetActor(
            IDiscordClient client,
            ulong id
        ) => GetActor(id);

        ITask<IReadOnlyCollection<IGuildChannel>> ILinkType<
            IGuildChannelActor,
            ulong,
            IGuildChannel,
            IGuildChannelModel
        >.Enumerable.AllAsync(
            RequestOptions? options,
            CancellationToken token
        ) => AllAsync(options, token);

        IGuildChannelActor ILinkType<
            IGuildChannelActor,
            ulong,
            IGuildChannel,
            IGuildChannelModel
        >.Indexable.Specifically(ulong id)
            => Specifically(id);

        IGuildActor IBackLink<IGuildActor, IGuildChannelActor, ulong, IGuildChannel, IGuildChannelModel>.Source
            => Source;
    }

    #endregion
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
        (DefaultMessageNotifications) Model.DefaultMessageNotifications;

    public MfaLevel MfaLevel => (MfaLevel) Model.MFALevel;

    public new GuildFeatures Features => base.Features ?? default;

    public PremiumTier PremiumTier => (PremiumTier) Model.PremiumTier;

    public SystemChannelFlags SystemChannelFlags => (SystemChannelFlags) Model.SystemChannelFlags;

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

    public ExplicitContentFilterLevel ExplicitContentFilter => (ExplicitContentFilterLevel) Model.ExplicitContentFilter;

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
            IModelSourceOfMultiple<ICustomEmoteModel> emotes
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