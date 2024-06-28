using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Guilds.Integrations;
using System.Globalization;

namespace Discord.Rest.Guilds;

public partial class RestLoadableGuildActor(DiscordRestClient client, GuildIdentity guild) :
    RestGuildActor(client, guild.Id),
    ILoadableGuildActor
{
    internal GuildIdentity Identity { get; } = guild;

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
public partial class RestGuildActor(DiscordRestClient client, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild) :
    RestActor<ulong, RestGuild>(client, guild.Id),
    IGuildActor
{
    public RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IEnumerable<IIntegrationModel>> Integrations
    {
        get;
    } = RestActors.Integrations(client, id, guild);

    public RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams> Bans { get; }
        = RestActors.Bans(client, id);

    public IEnumerableIndexableActor<ILoadableStageChannelActor, ulong, IStageChannel> StageChannels { get; }
        = RestActors.StageChannels(client, guild);

    public IEnumerableIndexableActor<ILoadableThreadChannelActor, ulong, IThreadChannel> ActiveThreads => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableTextChannelActor, ulong, ITextChannel> TextChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableGuildChannelActor, ulong, IGuildChannel> Channels => throw new NotImplementedException();

    public RestPagedIndexableActor<RestLoadableGuildMemberActor, ulong, RestGuildMember, IEnumerable<IMemberModel>,
        PageGuildMembersParams> Members
    {
        get;
    } = RestActors.Members(client, id);

    public IEnumerableIndexableActor<ILoadableGuildEmoteActor, ulong, IGuildEmote> Emotes => throw new NotImplementedException();

    public IEnumerableIndexableActor<IRoleActor, ulong, IRole> Roles => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableGuildStickerActor, ulong, IGuildSticker> Stickers => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents => throw new NotImplementedException();

    public IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites => throw new NotImplementedException();

    IPagedIndexableActor<ILoadableGuildBanActor, ulong, IBan, PageGuildBansParams> IGuildActor.Bans => Bans;

    IPagedIndexableActor<ILoadableGuildMemberActor, ulong, IGuildMember, PageGuildMembersParams> IGuildActor.Members =>
        Members;

    IEnumerableIndexableActor<IIntegrationActor, ulong, IIntegration> IGuildActor.Integrations => Integrations;
}

public partial class RestGuild(DiscordRestClient client, IGuildModel model, RestGuildActor? actor = null) :
    RestPartialGuild(client, model),
    IGuild,
    IConstructable<RestGuild, IGuildModel, DiscordRestClient>
{
    protected override IGuildModel Model { get; } = model;

    [ProxyInterface(typeof(IGuildActor))]
    protected virtual RestGuildActor Actor { get; } = actor ?? new(client, model.Id);

    public static RestGuild Construct(DiscordRestClient client, IGuildModel model)
        => new(client, model);

    #region Loadables

    public RestLoadableTextChannelActor? AFKChannel { get; } = model.AFKChannelId.HasValue
        ? new(client, model.Id, model.AFKChannelId.Value)
        : null;

    public RestLoadableTextChannelActor? WidgetChannel { get; } = model.WidgetChannelId.HasValue
        ? new(client, model.Id, model.WidgetChannelId.Value)
        : null;

    public RestLoadableTextChannelActor? SafetyAlertsChannel { get; } = model.SafetyAlertsChannelId.HasValue
        ? new(client, model.Id, model.SafetyAlertsChannelId.Value)
        : null;

    public RestLoadableTextChannelActor? SystemChannel { get; } = model.SystemChannelId.HasValue
        ? new(client, model.Id, model.SystemChannelId.Value)
        : null;

    public RestLoadableTextChannelActor? RulesChannel { get; } = model.RulesChannelId.HasValue
        ? new(client, model.Id, model.RulesChannelId.Value)
        : null;

    public RestLoadableTextChannelActor? PublicUpdatesChannel { get; } = model.PublicUpdatesChannelId.HasValue
        ? new(client, model.Id, model.PublicUpdatesChannelId.Value)
        : null;

    public RestLoadableGuildMemberActor Owner { get; } = new(client, model.Id, model.OwnerId);

    #endregion

    #region Properties

    public int AFKTimeout => Model.AFKTimeout;

    public bool IsWidgetEnabled => Model.WidgetEnabled;

    public DefaultMessageNotifications DefaultMessageNotifications => (DefaultMessageNotifications)Model.DefaultMessageNotifications;

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

    #endregion

    ILoadableTextChannelActor? IGuild.AFKChannel => AFKChannel;
    ILoadableTextChannelActor? IGuild.WidgetChannel => WidgetChannel;
    ILoadableTextChannelActor? IGuild.SafetyAlertsChannel => SafetyAlertsChannel;
    ILoadableTextChannelActor? IGuild.SystemChannel => SystemChannel;
    ILoadableTextChannelActor? IGuild.RulesChannel => RulesChannel;
    ILoadableTextChannelActor? IGuild.PublicUpdatesChannel => PublicUpdatesChannel;
    ILoadableGuildMemberActor IGuild.Owner => Owner;
}
