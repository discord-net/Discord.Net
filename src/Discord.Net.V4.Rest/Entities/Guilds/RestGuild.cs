using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;
using System.Globalization;
using MorseCode.ITask;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestGuildActor :
    RestActor<RestGuildActor, ulong, RestGuild, IGuildModel>,
    IGuildActor
{
    [SourceOfTruth] public RestGuildSoundboardSoundActor.Enumerable.Indexable Sounds { get; }

    [SourceOfTruth]
    public RestGuildTemplateFromGuildActor.Enumerable.Indexable.BackLink<RestGuildActor> Templates { get; }

    [SourceOfTruth]
    public RestGuildChannelActor.Enumerable.Indexable.Hierarchy.BackLink<RestGuildActor> Channels { get; }

    [SourceOfTruth] public RestGuildThreadChannelActor.Indexable.WithActive.BackLink<RestGuildActor> Threads { get; }
    [SourceOfTruth] public RestIntegrationActor.Enumerable.Indexable.BackLink<RestGuildActor> Integrations { get; }
    [SourceOfTruth] public RestBanActor.Paged<PageGuildBansParams>.Indexable.BackLink<RestGuildActor> Bans { get; }

    [SourceOfTruth]
    public RestMemberActor.Paged<PageGuildMembersParams>.Indexable.WithCurrent.BackLink<RestGuildActor> Members { get; }

    [SourceOfTruth] public RestGuildEmoteActor.Enumerable.Indexable.BackLink<RestGuildActor> Emotes { get; }
    [SourceOfTruth] public RestRoleActor.Enumerable.Indexable.BackLink<RestGuildActor> Roles { get; }
    [SourceOfTruth] public RestGuildStickerActor.Enumerable.Indexable.BackLink<RestGuildActor> Stickers { get; }

    [SourceOfTruth]
    public RestGuildScheduledEventActor.Enumerable.Indexable.BackLink<RestGuildActor> ScheduledEvents { get; }

    [SourceOfTruth] public RestGuildInviteActor.Enumerable.Indexable.BackLink<RestGuildActor> Invites { get; }
    [SourceOfTruth] public RestWebhookActor.Enumerable.Indexable.BackLink<RestGuildActor> Webhooks { get; }

    internal override GuildIdentity Identity { get; }

    [TypeFactory]
    public RestGuildActor(
        DiscordRestClient client,
        GuildIdentity guild
    ) : base(client, guild)
    {
        Identity = guild | this;

        Sounds = new(
            client,
            RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildSoundboardSoundIdentity>(), Identity),
            IGuildSoundboardSound.FetchManyRoute(this)
                .AsRequiredProvider()
                .ToEntityEnumerableProvider(
                    Discord.Template.Of<GuildSoundboardSoundIdentity>(),
                    RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildSoundboardSoundIdentity>(), Identity)
                )
        );

        Templates = new(
            this,
            client,
            RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildTemplateFromGuildIdentity>(), Identity),
            IGuildTemplate.FetchManyRoute(this)
                .AsRequiredProvider()
                .ToEntityEnumerableProvider(
                    Discord.Template.Of<GuildTemplateIdentity>(),
                    RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildTemplateFromGuildIdentity>(), Identity)
                )
        );
    }

    [SourceOfTruth]
    internal override RestGuild CreateEntity(IGuildModel model)
        => RestGuild.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestGuild :
    RestPartialGuild,
    IGuild,
    IRestConstructable<RestGuild, RestGuildActor, IGuildModel>
{
    [SourceOfTruth] public RestVoiceChannelActor? AFKChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? WidgetChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? SafetyAlertsChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? SystemChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? RulesChannel { get; private set; }

    [SourceOfTruth] public RestTextChannelActor? PublicUpdatesChannel { get; private set; }

    [SourceOfTruth] public RestMemberActor Owner { get; private set; }

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
        RestGuildActor actor
    ) : base(client, model)
    {
        _model = model;
        Actor = actor;

        var identity = GuildIdentity.Of(this);

        if (model is IModelSourceOfMultiple<IRoleModel> roles)
            actor.Roles.AddModelSources(Discord.Template.Of<RoleIdentity>(), roles);

        if (model is IModelSourceOfMultiple<ICustomEmoteModel> emotes)
            actor.Emotes.AddModelSources(Discord.Template.Of<GuildEmoteIdentity>(), emotes);

        if (model is IModelSourceOfMultiple<IGuildStickerModel> stickers)
            actor.Stickers.AddModelSources(Discord.Template.Of<GuildStickerIdentity>(), stickers);


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

    public static RestGuild Construct(DiscordRestClient client, RestGuildActor actor, IGuildModel model)
        => new(client, model, actor);

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