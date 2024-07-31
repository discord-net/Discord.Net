using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using System.Globalization;

namespace Discord.Gateway;

public sealed partial class GatewayGuildActor :
    GatewayCachedActor<ulong, GatewayGuild, GuildIdentity, IGuildModel>,
    IGuildActor
{
     public IEnumerableIndexableActor<IGuildChannelActor, ulong, IGuildChannel> Channels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ITextChannelActor, ulong, ITextChannel> TextChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<IVoiceChannelActor, ulong, IVoiceChannel> VoiceChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<ICategoryChannelActor, ulong, ICategoryChannel> CategoryChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<INewsChannelActor, ulong, INewsChannel> AnnouncementChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<IThreadChannelActor, ulong, IThreadChannel> ThreadChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<IThreadChannelActor, ulong, IThreadChannel> ActiveThreadChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<IStageChannelActor, ulong, IStageChannel> StageChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<IForumChannelActor, ulong, IForumChannel> ForumChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<IMediaChannelActor, ulong, IMediaChannel> MediaChannels => throw new NotImplementedException();

    public IEnumerableIndexableActor<IIntegrationActor, ulong, IIntegration> Integrations => throw new NotImplementedException();

    public IPagedIndexableActor<IBanActor, ulong, IBan, PageGuildBansParams> Bans => throw new NotImplementedException();

    public IPagedIndexableActor<IGuildMemberActor, ulong, IGuildMember, PageGuildMembersParams> Members => throw new NotImplementedException();

    public IEnumerableIndexableActor<IGuildEmoteActor, ulong, IGuildEmote> Emotes => throw new NotImplementedException();

    public IEnumerableIndexableActor<IRoleActor, ulong, IRole> Roles => throw new NotImplementedException();

    public IEnumerableIndexableActor<IGuildStickerActor, ulong, IGuildSticker> Stickers => throw new NotImplementedException();

    public IEnumerableIndexableActor<IGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents => throw new NotImplementedException();

    public IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites => throw new NotImplementedException();

    public IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();

    public GatewayGuildActor(DiscordGatewayClient client, GuildIdentity guild)
        : base(client, guild)
    {
    }

    [SourceOfTruth]
    internal GatewayGuild CreateEntity(IGuildModel model)
        => Client.StateController.CreateLatent(this, model);

    [SourceOfTruth]
    internal IGuildChannel CreateEntity(IGuildChannelModel model) => throw new NotImplementedException();

    [SourceOfTruth]
    internal IGuildMember CreateEntity(IMemberModel model) => throw new NotImplementedException();
}

public sealed partial class GatewayGuild :
    GatewayCacheableEntity<GatewayGuild, ulong, IGuildModel, GuildIdentity>,
    IGuild
{
    public string Name => throw new NotImplementedException();

    public string? SplashId => throw new NotImplementedException();

    public string? BannerId => throw new NotImplementedException();

    public string? Description => throw new NotImplementedException();

    public string? IconId => throw new NotImplementedException();

    public string? VanityUrlCode => throw new NotImplementedException();

    public int AFKTimeout => throw new NotImplementedException();

    public bool IsWidgetEnabled => throw new NotImplementedException();

    public DefaultMessageNotifications DefaultMessageNotifications => throw new NotImplementedException();

    public MfaLevel MfaLevel => throw new NotImplementedException();

    public GuildFeatures Features => throw new NotImplementedException();

    public PremiumTier PremiumTier => throw new NotImplementedException();

    public SystemChannelFlags SystemChannelFlags => throw new NotImplementedException();

    public NsfwLevel NsfwLevel => throw new NotImplementedException();

    public CultureInfo PreferredCulture => throw new NotImplementedException();

    public bool IsBoostProgressBarEnabled => throw new NotImplementedException();

    public int PremiumSubscriptionCount => throw new NotImplementedException();

    public int? MaxPresences => throw new NotImplementedException();

    public int? MaxMembers => throw new NotImplementedException();

    public int? MaxVideoChannelUsers => throw new NotImplementedException();

    public int? MaxStageVideoChannelUsers => throw new NotImplementedException();

    public int? ApproximateMemberCount => throw new NotImplementedException();

    public int? ApproximatePresenceCount => throw new NotImplementedException();

    public string PreferredLocale => throw new NotImplementedException();

    public VerificationLevel VerificationLevel => throw new NotImplementedException();

    public ExplicitContentFilterLevel ExplicitContentFilter => throw new NotImplementedException();

    public string? DiscoverySplashId => throw new NotImplementedException();

    public IVoiceChannelActor? AFKChannel => throw new NotImplementedException();

    public ITextChannelActor? WidgetChannel => throw new NotImplementedException();

    public ITextChannelActor? SafetyAlertsChannel => throw new NotImplementedException();

    public ITextChannelActor? SystemChannel => throw new NotImplementedException();

    public ITextChannelActor? RulesChannel => throw new NotImplementedException();

    public ITextChannelActor? PublicUpdatesChannel => throw new NotImplementedException();

    public IGuildMemberActor Owner => throw new NotImplementedException();

    public ulong? ApplicationId => throw new NotImplementedException();

    [ProxyInterface]
    internal GatewayGuildActor Actor { get; }

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
