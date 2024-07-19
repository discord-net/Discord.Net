using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds.Integrations;
using Discord.Rest.Stickers;
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
using EnumerableGuildScheduledEvent =
    RestEnumerableIndexableActor<RestGuildScheduledEventActor, ulong, RestGuildScheduledEvent, IGuildScheduledEvent,
        IEnumerable<IGuildScheduledEventModel>>;

[ExtendInterfaceDefaults(typeof(IGuildActor))]
public partial class RestGuildActor :
    RestActor<ulong, RestGuild, GuildIdentity>,
    IGuildActor
{
    [TypeFactory]
    public RestGuildActor(
        DiscordRestClient client,
        GuildIdentity guild
    ) : base(client, guild)
    {
        MediaChannels = RestActors.GuildRelatedEntity<RestMediaChannelActor>(client, this);
        Channels = RestActors.GuildRelatedEntity<RestGuildChannelActor>(client, this);
        TextChannels = RestActors.GuildRelatedEntity<RestTextChannelActor>(client, this);
        VoiceChannels = RestActors.GuildRelatedEntity<RestVoiceChannelActor>(client, this);
        CategoryChannels = RestActors.GuildRelatedEntity<RestCategoryChannelActor>(client, this);
        AnnouncementChannels = RestActors.GuildRelatedEntity<RestNewsChannelActor>(client, this);
        ThreadChannels = RestActors.GuildRelatedEntity<RestThreadChannelActor>(client, this);
        StageChannels = RestActors.GuildRelatedEntity<RestStageChannelActor>(client, this);
        ForumChannels = RestActors.GuildRelatedEntity<RestForumChannelActor>(client, this);
        ActiveThreadChannels =
            RestActors.GuildRelatedEntityWithTransform<RestThreadChannelActor, ListActiveGuildThreadsResponse>(
                client,
                this,
                Routes.ListActiveGuildThreads(guild.Id),
                api => api.Threads
            );
        Integrations = RestActors.GuildRelatedEntity<RestIntegrationActor>(client, this);
        Bans = RestActors.Bans(client, guild);
        Members = RestActors.Members(client, guild);
        Emotes = RestActors.GuildRelatedEntity<RestGuildEmoteActor>(client, this);
        Roles = RestActors.GuildRelatedEntity<RestRoleActor>(client, this);
        Stickers = RestActors.GuildRelatedEntity<RestGuildStickerActor>(client, this);
        ScheduledEvents = RestActors.GuildRelatedEntity<RestGuildScheduledEventActor>(client, this);
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

    [SourceOfTruth] public EnumerableGuildScheduledEvent ScheduledEvents { get; }

    public IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites =>
        throw new NotImplementedException();

    [SourceOfTruth]
    internal RestGuild CreateEntity(IGuildModel model)
        => RestGuild.Construct(Client, model);
}

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

    //public RestManagedEnumerableActor<RestRoleActor,>

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

    internal RestGuild(DiscordRestClient client,
        IGuildModel model,
        RestGuildActor? actor = null) : base(client, model)
    {
        _model = model;
        Actor = actor ?? new(client, GuildIdentity.Of(this));

        if (model is not IModelSourceOfMultiple<IRoleModel> roles)
            throw new ArgumentException($"{nameof(model)} must provide a collection of {nameof(IRoleModel)}.");

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
