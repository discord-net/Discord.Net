using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds.Integrations;
using System.Globalization;

namespace Discord.Rest.Guilds;

public partial class RestLoadableGuildActor(
    DiscordRestClient client,
    GuildIdentity guild
) :
    RestGuildActor(client, guild),
    ILoadableGuildActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuild>))]
    internal RestLoadable<ulong, RestGuild, IGuild, IGuildModel> Loadable { get; } =
        RestLoadable<ulong, RestGuild, IGuild, IGuildModel>.FromConstructable<RestGuild>(
            client,
            guild,
            Routes.GetGuild(guild.Id)
        );
}

[ExtendInterfaceDefaults(
    typeof(IGuildActor),
    typeof(IModifiable<ulong, IGuildActor, ModifyGuildProperties, ModifyGuildParams>),
    typeof(IDeletable<ulong, IGuildActor>)
)]
public partial class RestGuildActor(
    DiscordRestClient client,
    GuildIdentity guild
) :
    RestActor<ulong, RestGuild, GuildIdentity>(client, guild),
    IGuildActor
{
    public
    public IEnumerableIndexableActor<ILoadableMediaChannelActor, ulong, IMediaChannel> MediaChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableGuildChannelActor, ulong, IGuildChannel> Channels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableTextChannelActor, ulong, ITextChannel> TextChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableVoiceChannelActor, ulong, IVoiceChannel> VoiceChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableCategoryChannelActor, ulong, ICategoryChannel> CategoryChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableNewsChannelActor, ulong, INewsChannel> AnnouncementChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableThreadChannelActor, ulong, IThreadChannel> ThreadChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableThreadChannelActor, ulong, IThreadChannel> ActiveThreadChannels => throw new NotImplementedException();

    public RestEnumerableIndexableIntegrationActor Integrations
    {
        get;
    } = RestActors.Integrations(client, guild);

    public RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams>
        Bans { get; }
        = RestActors.Bans(client, guild);

    public RestEnumerableIndexableStageChannelActor StageChannels { get; }
        = RestActors.StageChannels(client, guild);

    public IEnumerableIndexableActor<ILoadableForumChannelActor, ulong, IForumChannel> ForumChannels => throw new NotImplementedException();


    public RestPagedIndexableActor<RestLoadableGuildMemberActor, ulong, RestGuildMember, IEnumerable<IMemberModel>,
        PageGuildMembersParams> Members
    {
        get;
    } = RestActors.Members(client, guild);

    public IEnumerableIndexableActor<ILoadableGuildEmoteActor, ulong, IGuildEmote> Emotes =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IRoleActor, ulong, IRole> Roles => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableGuildStickerActor, ulong, IGuildSticker> Stickers =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites =>
        throw new NotImplementedException();

    IPagedIndexableActor<ILoadableGuildBanActor, ulong, IBan, PageGuildBansParams> IGuildActor.Bans => Bans;

    IPagedIndexableActor<ILoadableGuildMemberActor, ulong, IGuildMember, PageGuildMembersParams> IGuildActor.Members =>
        Members;

    IEnumerableIndexableActor<IIntegrationActor, ulong, IIntegration> IGuildActor.Integrations => Integrations;
    IEnumerableIndexableActor<ILoadableStageChannelActor, ulong, IStageChannel> IGuildActor.StageChannels => StageChannels;

    IGuild IEntityProvider<IGuild, IGuildModel>.CreateEntity(IGuildModel model)
        => RestGuild.Construct(Client, model);
}

public sealed partial class RestGuild :
    RestPartialGuild,
    IGuild,
    IConstructable<RestGuild, IGuildModel, DiscordRestClient>
{
    public RestLoadableVoiceChannelActor? AFKChannel { get; private set; }

    public RestLoadableTextChannelActor? WidgetChannel { get; private set;}

    public RestLoadableTextChannelActor? SafetyAlertsChannel { get; private set;}

    public RestLoadableTextChannelActor? SystemChannel { get; private set;}

    public RestLoadableTextChannelActor? RulesChannel { get; private set;}

    public RestLoadableTextChannelActor? PublicUpdatesChannel { get; private set;}

    public RestLoadableGuildMemberActor Owner { get; private set; }

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

        AFKChannel = model.AFKChannelId
            .Map(
                static (id, client, identity) => new RestLoadableVoiceChannelActor(
                    client,
                    identity,
                    VoiceChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        WidgetChannel = model.WidgetChannelId
            .Map(
                static (id, client, identity) => new RestLoadableTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        SafetyAlertsChannel = model.SafetyAlertsChannelId
            .Map(
                static (id, client, identity) => new RestLoadableTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        SystemChannel = model.SystemChannelId
            .Map(
                static (id, client, identity) => new RestLoadableTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        RulesChannel = model.RulesChannelId
            .Map(
                static (id, client, identity) => new RestLoadableTextChannelActor(
                    client,
                    identity,
                    TextChannelIdentity.Of(id)
                ),
                client,
                Actor.Identity
            );

        PublicUpdatesChannel = model.PublicUpdatesChannelId
            .Map(
                static (id, client, identity) => new RestLoadableTextChannelActor(
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
            RestLoadableVoiceChannelActor.Factory,
            VoiceChannelIdentity.Of,
            Client,
            Actor.Identity
        );

        WidgetChannel = WidgetChannel.UpdateFrom(
            model.WidgetChannelId,
            RestLoadableTextChannelActor.Factory,
            TextChannelIdentity.Of,
            Client,
            Actor.Identity
        );

        SafetyAlertsChannel = SafetyAlertsChannel.UpdateFrom(
            model.SafetyAlertsChannelId,
            RestLoadableTextChannelActor.Factory,
            TextChannelIdentity.Of,
            Client,
            Actor.Identity
        );

        SystemChannel = SystemChannel.UpdateFrom(
            model.SystemChannelId,
            RestLoadableTextChannelActor.Factory,
            TextChannelIdentity.Of,
            Client,
            Actor.Identity
        );

        RulesChannel = RulesChannel.UpdateFrom(
            model.RulesChannelId,
            RestLoadableTextChannelActor.Factory,
            TextChannelIdentity.Of,
            Client,
            Actor.Identity
        );

        PublicUpdatesChannel = PublicUpdatesChannel.UpdateFrom(
            model.PublicUpdatesChannelId,
            RestLoadableTextChannelActor.Factory,
            TextChannelIdentity.Of,
            Client,
            Actor.Identity
        );

        Owner = Owner.UpdateFrom(
            model.OwnerId,
            RestLoadableGuildMemberActor.Factory,
            MemberIdentity.Of,
            Client,
            Actor.Identity
        );

        _model = model;

        return ValueTask.CompletedTask;
    }

    public IGuildModel GetModel() => Model;

    ILoadableVoiceChannelActor? IGuild.AFKChannel => AFKChannel;
    ILoadableTextChannelActor? IGuild.WidgetChannel => WidgetChannel;
    ILoadableTextChannelActor? IGuild.SafetyAlertsChannel => SafetyAlertsChannel;
    ILoadableTextChannelActor? IGuild.SystemChannel => SystemChannel;
    ILoadableTextChannelActor? IGuild.RulesChannel => RulesChannel;
    ILoadableTextChannelActor? IGuild.PublicUpdatesChannel => PublicUpdatesChannel;
    ILoadableGuildMemberActor IGuild.Owner => Owner;
}
