using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using System.Globalization;

namespace Discord.Gateway;

public sealed partial class GatewayGuildActor :
    GatewayCachedActor<ulong, GatewayGuild, GuildIdentity, IGuildModel>,
    IGuildActor
{
    public IEnumerableIndexableActor<IGuildChannelActor, ulong, IGuildChannel> Channels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<ITextChannelActor, ulong, ITextChannel> TextChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IVoiceChannelActor, ulong, IVoiceChannel> VoiceChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<ICategoryChannelActor, ulong, ICategoryChannel> CategoryChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<INewsChannelActor, ulong, INewsChannel> AnnouncementChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IThreadChannelActor, ulong, IThreadChannel> ThreadChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IThreadChannelActor, ulong, IThreadChannel> ActiveThreadChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IStageChannelActor, ulong, IStageChannel> StageChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IForumChannelActor, ulong, IForumChannel> ForumChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IMediaChannelActor, ulong, IMediaChannel> MediaChannels =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IIntegrationActor, ulong, IIntegration> Integrations =>
        throw new NotImplementedException();

    public IPagedIndexableActor<IBanActor, ulong, IBan, PageGuildBansParams> Bans =>
        throw new NotImplementedException();

    public IPagedIndexableActor<IGuildMemberActor, ulong, IGuildMember, PageGuildMembersParams> Members =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IGuildEmoteActor, ulong, IGuildEmote> Emotes =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IRoleActor, ulong, IRole> Roles => throw new NotImplementedException();

    public IEnumerableIndexableActor<IGuildStickerActor, ulong, IGuildSticker> Stickers =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents =>
        throw new NotImplementedException();

    public IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites => throw new NotImplementedException();

    public IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();

    public GatewayGuildActor(DiscordGatewayClient client, GuildIdentity guild)
        : base(client, guild)
    {
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
    GatewayCacheableEntity<GatewayGuild, ulong, IGuildModel, GuildIdentity>,
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
        GatewayGuildActor? actor = null,
        IEntityHandle<ulong, GatewayGuild>? implicitHandle = null
    ) : base(client, model.Id, implicitHandle)
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
        ICacheConstructionContext<ulong, GatewayGuild> context,
        IGuildModel model)
    {
        return new GatewayGuild(
            client,
            model,
            context.TryGetActor(Template.T<GatewayGuildActor>()),
            context.ImplicitHandle
        );
    }
}
