using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord;

[Loadable(nameof(Routes.GetGuild))]
[Modifiable<ModifyGuildProperties>(nameof(Routes.ModifyGuild))]
[Deletable(nameof(Routes.DeleteGuild))]
public partial interface IGuildActor :
    IActor<ulong, IGuild>,
    IContainsThreadsTrait<IGuildThreadChannelActor>,
    IInvitableTrait<IGuildInviteActor, IGuildInvite>,
    IEntityProvider<IGuildChannel, IGuildChannelModel>,
    IEntityProvider<IMember, IMemberModel>
{
    ICurrentMemberActor CurrentMember { get; }

    #region Channels

    [return: TypeHeuristic(nameof(Channels))]
    IGuildChannelActor Channel(ulong id) => Channels[id];
    GuildChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Channels { get; }

    [return: TypeHeuristic(nameof(TextChannels))]
    ITextChannelActor TextChannel(ulong id) => TextChannels[id];
    TextChannelLink.Enumerable.Indexable.BackLink<IGuildActor> TextChannels { get; }

    [return: TypeHeuristic(nameof(VoiceChannels))]
    IVoiceChannelActor VoiceChannel(ulong id) => VoiceChannels[id];
    VoiceChannelLink.Enumerable.Indexable.BackLink<IGuildActor> VoiceChannels { get; }

    [return: TypeHeuristic(nameof(CategoryChannels))]
    ICategoryChannelActor CategoryChannel(ulong id) => CategoryChannels[id];
    CategoryChannelLink.Enumerable.Indexable.BackLink<IGuildActor> CategoryChannels { get; }

    [return: TypeHeuristic(nameof(AnnouncementChannels))]
    INewsChannelActor AnnouncementChannel(ulong id) => AnnouncementChannels[id];
    NewsChannelLink.Enumerable.Indexable.BackLink<IGuildActor> AnnouncementChannels { get; }
    
    [return: TypeHeuristic(nameof(StageChannels))]
    IStageChannelActor StageChannel(ulong id) => StageChannels[id];
    StageChannelLink.Enumerable.Indexable.BackLink<IGuildActor> StageChannels { get; }

    [return: TypeHeuristic(nameof(ForumChannels))]
    IForumChannelActor ForumChannel(ulong id) => ForumChannels[id];
    ForumChannelLink.Enumerable.Indexable.BackLink<IGuildActor> ForumChannels { get; }

    [return: TypeHeuristic(nameof(MediaChannels))]
    IMediaChannelActor MediaChannel(ulong id) => MediaChannels[id];
    MediaChannelLink.Enumerable.Indexable.BackLink<IGuildActor> MediaChannels { get; }

    [return: TypeHeuristic(nameof(IntegrationChannels))]
    IIntegrationChannelTrait IntegrationChannel(ulong id) => IntegrationChannels[id];
    IntegrationChannelTraitLink.Enumerable.Indexable.BackLink<IGuildActor> IntegrationChannels { get; }

    [SourceOfTruth]
    new ThreadsLink Threads { get; }
    
    public interface ThreadsLink :
        GuildThreadChannelLink.Indexable
    {
        GuildThreadChannelLink.Enumerable Active { get; }
    }
    
    #endregion

    [return: TypeHeuristic(nameof(Integrations))]
    IIntegrationActor Integration(ulong id) => Integrations[id];
    IntegrationLink.Enumerable.Indexable Integrations { get; }

    [return: TypeHeuristic(nameof(Bans))]
    IBanActor Ban(ulong userId) => Bans[userId];
    BanLink.Paged<PageGuildBansParams>.Indexable.BackLink<IGuildActor> Bans { get; }

    [return: TypeHeuristic(nameof(Members))]
    IMemberActor Member(ulong id) => Members[id];
    MemberLink.Paged<PageGuildMembersParams>.Indexable.BackLink<IGuildActor> Members { get; }

    [return: TypeHeuristic(nameof(Emotes))]
    IGuildEmoteActor Emote(ulong id) => Emotes[id];
    GuildEmoteLink.Enumerable.Indexable.BackLink<IGuildActor> Emotes { get; }

    [return: TypeHeuristic(nameof(Roles))]
    IRoleActor Role(ulong id) => Roles[id];
    RoleLink.Enumerable.Indexable.BackLink<IGuildActor> Roles { get; }

    [return: TypeHeuristic(nameof(Stickers))]
    IGuildStickerActor Sticker(ulong id) => Stickers[id];
    GuildStickerLink.Enumerable.Indexable.BackLink<IGuildActor> Stickers { get; }

    [return: TypeHeuristic(nameof(ScheduledEvents))]
    IGuildScheduledEventActor ScheduledEvent(ulong id) => ScheduledEvents[id];
    GuildScheduledEventLink.Enumerable.Indexable.BackLink<IGuildActor> ScheduledEvents { get; }

    [return: TypeHeuristic(nameof(Webhooks))]
    IWebhookActor Webhook(ulong id) => Webhooks[id];
    WebhookLink.Enumerable.Indexable Webhooks { get; }

    #region Methods

    Task LeaveAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return Client.RestApiClient.ExecuteAsync(
            Routes.LeaveGuild(Id),
            options ?? Client.DefaultRequestOptions,
            token
        );
    }
    
    async Task<MfaLevel> ModifyMFALevelAsync(
        MfaLevel level,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.ModifyGuildMfaLevel(Id, new ModifyGuildMfaLevelParams {Level = (int)level}),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return (MfaLevel)result.Level;
    }

    async Task<int> GetPruneCountAsync(
        int? days = null,
        Optional<IEnumerable<EntityOrId<ulong, IRole>>> includeRoles = default,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetGuildPruneCount(
                Id,
                days,
                ~includeRoles.Map(v => v.Select(v => v.Id).ToArray())
            ),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result.Pruned;
    }

    async Task<int?> BeginPruneAsync(
        int? days = null,
        bool? computePruneCount = null,
        Optional<IEnumerable<EntityOrId<ulong, IRole>>> includeRoles = default,
        string? reason = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteAsync(
            Routes.BeginGuildPrune(Id,
                new BeginGuildPruneParams
                {
                    Days = Optional.FromNullable(days),
                    Reason = Optional.FromNullable(reason),
                    ComputePruneCount = Optional.FromNullable(computePruneCount),
                    IncludeRoleIds = includeRoles.Map(v => v.Select(v => v.Id).ToArray())
                }),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result?.Pruned;
    }

    async Task<IReadOnlyCollection<VoiceRegion>> GetGuildVoiceRegionsAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetGuildVoiceRegions(Id),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result.Select(v => VoiceRegion.Construct(Client, v)).ToImmutableArray();
    }

    #endregion
}
