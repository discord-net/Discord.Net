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
        
        Emotes = RestGuildEmoteActor.Enumerable.Indexable.BackLink<RestGuildActor>.Create(
            this,
            RestGuildEmoteActor.DefaultEnumerableProvider,
            client,
            RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildEmoteIdentity>(), Identity)
        );

        // Sounds = new(
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildSoundboardSoundIdentity>(), Identity),
        //     IGuildSoundboardSound.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<GuildSoundboardSoundIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildSoundboardSoundIdentity>(), Identity)
        //         )
        // );
        //
        // Templates = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildTemplateFromGuildIdentity>(), Identity),
        //     IGuildTemplate.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<GuildTemplateIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildTemplateFromGuildIdentity>(),
        //                 Identity)
        //         )
        // );
        //
        // Channels = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildChannelIdentity>(), Identity),
        //     IGuildChannel.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<GuildChannelIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildChannelIdentity>(), Identity)
        //         ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<CategoryChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IGuildCategoryChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<CategoryChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<CategoryChannelIdentity>(), Identity)
        //             )
        //     ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<ForumChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IGuildForumChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<ForumChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<ForumChannelIdentity>(), Identity)
        //             )
        //     ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<MediaChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IGuildMediaChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<MediaChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<MediaChannelIdentity>(), Identity)
        //             )
        //     ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<NewsChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IGuildNewsChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<NewsChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<NewsChannelIdentity>(), Identity)
        //             )
        //     ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<StageChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IGuildStageChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<StageChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<StageChannelIdentity>(), Identity)
        //             )
        //     ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<TextChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IGuildTextChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<TextChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<TextChannelIdentity>(), Identity)
        //             )
        //     ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<ThreadableChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IThreadableChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<ThreadableChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<ThreadableChannelIdentity>(),
        //                     Identity)
        //             )
        //     ),
        //     new(
        //         this,
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<VoiceChannelIdentity>(), Identity),
        //         IGuildChannel.FetchManyRoute(this)
        //             .AsRequiredProvider()
        //             .Map(v => v.OfType<IGuildVoiceChannelModel>())
        //             .ToEntityEnumerableProvider(
        //                 Discord.Template.Of<VoiceChannelIdentity>(),
        //                 RestActorProvider.GetOrCreate(client, Discord.Template.Of<VoiceChannelIdentity>(), Identity)
        //             )
        //     )
        // );
        //
        // Threads = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildThreadIdentity>(), Identity),
        //     new(
        //         client,
        //         RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildThreadIdentity>(), Identity),
        //         Routes.ListActiveGuildThreads(Id)
        //             .AsRequiredProvider()
        //             .ToEntityEnumerableProvider(result =>
        //             {
        //                 foreach (var member in result.Members)
        //                 {
        //                     if (!member.ThreadId.IsSpecified) continue;
        //                     Threads![member.ThreadId.Value].Members.Current.AddModelSource(member);
        //                 }
        //
        //                 return result.Threads.Select(model => Threads![model.Id].CreateEntity(model));
        //             })
        //     )
        // );
        //
        // Integrations = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<IntegrationIdentity>(), Identity),
        //     IIntegration.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<IntegrationIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<IntegrationIdentity>(), Identity)
        //         )
        // );
        //
        // Bans = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<BanIdentity>(), Identity),
        //     new RestPagingProvider<IBanModel, PageGuildBansParams, RestBan>(
        //         client,
        //         (model, _) => Bans![model.Id].CreateEntity(model),
        //         this
        //     )
        // );
        //
        // Members = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<MemberIdentity>(), Identity),
        //     new RestPagingProvider<IMemberModel, PageGuildMembersParams, RestMember>(
        //         client,
        //         (model, _) => Members![model.Id].CreateEntity(model),
        //         this
        //     ),
        //     new(client, Identity, CurrentMemberIdentity.Of(client.Users.Current.Id))
        // );
        //
        // Emotes = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildEmoteIdentity>(), Identity),
        //     IGuildEmote.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<GuildEmoteIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildEmoteIdentity>(), Identity)
        //         )
        // );
        //
        // Roles = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<RoleIdentity>(), Identity),
        //     IRole.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<RoleIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<RoleIdentity>(), Identity)
        //         )
        // );
        //
        // Stickers = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildStickerIdentity>(), Identity),
        //     IGuildSticker.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<GuildStickerIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildStickerIdentity>(), Identity)
        //         )
        // );
        //
        // ScheduledEvents = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildScheduledEventIdentity>(), Identity),
        //     IGuildScheduledEvent.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<GuildScheduledEventIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildScheduledEventIdentity>(), Identity)
        //         )
        // );
        //
        // Invites = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildInviteIdentity>(), Identity),
        //     IGuildInvite.FetchManyRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<GuildInviteIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<GuildInviteIdentity>(), Identity)
        //         )
        // );
        //
        // Webhooks = new(
        //     this,
        //     client,
        //     RestActorProvider.GetOrCreate(client, Discord.Template.Of<WebhookIdentity>()),
        //     IWebhook.GetGuildWebhooksRoute(this)
        //         .AsRequiredProvider()
        //         .ToEntityEnumerableProvider(
        //             Discord.Template.Of<WebhookIdentity>(),
        //             RestActorProvider.GetOrCreate(client, Discord.Template.Of<WebhookIdentity>())
        //         )
        // );
    }

    [SourceOfTruth]
    internal override RestGuild CreateEntity(IGuildModel model)
        => RestGuild.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestGuild :
    RestPartialGuild,
    IGuild,
    IRestEntity<ulong, IGuildModel>,
    IRestConstructable<RestGuild, RestGuildActor, IGuildModel>
{
    [SourceOfTruth]
    public RestVoiceChannelActor? AFKChannel
        => Computed(nameof(AFKChannel), m => m.AFKChannelId, model => 
            model.AFKChannelId.HasValue
                ? Actor.Channels.Voice[model.AFKChannelId.Value]
                : null
        );

    [SourceOfTruth]
    public RestTextChannelActor? WidgetChannel
        => Computed(nameof(WidgetChannel), m => m.WidgetChannelId, model => 
            model.WidgetChannelId.HasValue
                ? Actor.Channels.Text[model.WidgetChannelId.Value]
                : null
        );

    [SourceOfTruth] public RestTextChannelActor? SafetyAlertsChannel 
        => Computed(nameof(SafetyAlertsChannel), m => m.SafetyAlertsChannelId, model => 
            model.SafetyAlertsChannelId.HasValue
                ? Actor.Channels.Text[model.SafetyAlertsChannelId.Value]
                : null
        );

    [SourceOfTruth] public RestTextChannelActor? SystemChannel 
        => Computed(nameof(SystemChannel), m => m.SystemChannelId, model => 
            model.SystemChannelId.HasValue
                ? Actor.Channels.Text[model.SystemChannelId.Value]
                : null
        );

    [SourceOfTruth] public RestTextChannelActor? RulesChannel 
        => Computed(nameof(RulesChannel), m => m.RulesChannelId, model => 
            model.RulesChannelId.HasValue
                ? Actor.Channels.Text[model.RulesChannelId.Value]
                : null
        );

    [SourceOfTruth] public RestTextChannelActor? PublicUpdatesChannel 
        => Computed(nameof(PublicUpdatesChannel), m => m.PublicUpdatesChannelId, model => 
            model.PublicUpdatesChannelId.HasValue
                ? Actor.Channels.Text[model.PublicUpdatesChannelId.Value]
                : null
        );

    [SourceOfTruth]
    public RestMemberActor Owner
        => Computed(nameof(Owner), m => m.OwnerId, model => 
            Actor.Members[model.OwnerId]
        );

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

    [SourceOfTruth]
    public RestRoleActor.Defined.Indexable.BackLink<RestGuildActor> Roles { get; }
    
    [SourceOfTruth]
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

        Roles = new(
            actor,
            client,
            actor.Roles,
            model.RoleIds.ToList().AsReadOnly()
        );

        if (model is IModelSourceOfMultiple<IRoleModel> roles)
            actor.Roles.AddModelSources(Discord.Template.Of<RoleIdentity>(), roles);

        if (model is IModelSourceOfMultiple<ICustomEmoteModel> emotes)
            actor.Emotes.AddModelSources(Discord.Template.Of<GuildEmoteIdentity>(), emotes);

        if (model is IModelSourceOfMultiple<IGuildStickerModel> stickers)
        {
            foreach (var sticker in stickers.GetModels())
                actor.Stickers[model.Id].AddModelSource(sticker);
        }
    }

    public static RestGuild Construct(DiscordRestClient client, RestGuildActor actor, IGuildModel model)
        => new(client, model, actor);

    public async ValueTask UpdateAsync(IGuildModel model, CancellationToken token = default)
    {
        if (!Model.RoleIds.SequenceEqual(model.RoleIds))
            Roles.DefinedLink.Ids = model.RoleIds.ToList().AsReadOnly();
        
        if (model is IModelSourceOfMultiple<IRoleModel> roles)
        {
            foreach (var role in roles.GetModels())
                await Roles[role.Id].AddModelSourceAsync(role, token);
        }

        _model = model;
    }

    public override IGuildModel GetModel() => Model;
}