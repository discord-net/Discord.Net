using Discord.Integration;
using Discord.Models;
using Discord.Models.Json;
using System.Globalization;

namespace Discord.Rest.Guilds;

public partial class RestLoadableGuildActor(DiscordRestClient client, ulong id) :
    RestGuildActor(client, id),
    ILoadableGuildActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuild>))]
    internal RestLoadable<ulong, RestGuild, IGuild, Guild> Loadable { get; } =
        RestLoadable<ulong, RestGuild, IGuild, Guild>.FromConstructable<RestGuild>(client, id, Routes.GetGuild(id));
}

[ExtendInterfaceDefaults(
    typeof(IGuildActor),
    typeof(IModifiable<ulong, IGuildActor, ModifyGuildProperties, ModifyGuildParams>),
    typeof(IDeletable<ulong, IGuildActor>)
)]
public partial class RestGuildActor(DiscordRestClient client, ulong id) :
    RestActor<ulong, RestGuild>(client, id),
    IGuildActor
{
    public IRootActor<IIntegrationActor, ulong, IIntegration> Integrations => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableGuildBanActor, ulong, IBan> Bans => throw new NotImplementedException();

    public IRootActor<ILoadableStageChannelActor, ulong, IStageChannel> StageChannels => throw new NotImplementedException();

    public ILoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> ActiveThreads => throw new NotImplementedException();

    public IRootActor<ILoadableTextChannelActor, ulong, ITextChannel> TextChannels => throw new NotImplementedException();

    public ILoadableRootActor<ILoadableGuildChannelActor, ulong, IGuildChannel> Channels => throw new NotImplementedException();

    public IPagedLoadableRootActor<ILoadableGuildMemberActor, ulong, IGuildMember> Members => throw new NotImplementedException();

    public ILoadableRootActor<ILoadableGuildEmoteActor, ulong, IGuildEmote> Emotes => throw new NotImplementedException();

    public ILoadableRootActor<IRoleActor, ulong, IRole> Roles => throw new NotImplementedException();

    public ILoadableRootActor<ILoadableGuildStickerActor, ulong, IGuildSticker> Stickers => throw new NotImplementedException();

    public ILoadableRootActor<ILoadableGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents => throw new NotImplementedException();

    public ILoadableRootActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites => throw new NotImplementedException();
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

    public ILoadableEntity<ulong, IVoiceChannel> AFKChannel => throw new NotImplementedException();

    public ILoadableEntity<ulong, ITextChannel> WidgetChannel => throw new NotImplementedException();

    public ILoadableEntity<ulong, ITextChannel> SafetyAlertsChannel => throw new NotImplementedException();

    public ILoadableEntity<ulong, ITextChannel> SystemChannel => throw new NotImplementedException();

    public ILoadableEntity<ulong, ITextChannel> RulesChannel => throw new NotImplementedException();

    public ILoadableEntity<ulong, ITextChannel> PublicUpdatesChannel => throw new NotImplementedException();

    public ILoadableEntity<ulong, IGuildMember> Owner => throw new NotImplementedException();
    public ILoadableEntity<ulong, IRole> EveryoneRole => throw new NotImplementedException();

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
}
