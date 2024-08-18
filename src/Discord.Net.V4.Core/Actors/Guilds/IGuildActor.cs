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
    IEntityProvider<IGuildChannel, IGuildChannelModel>,
    IEntityProvider<IMember, IMemberModel>
{
    ICurrentMemberActor CurrentMember { get; }

    #region Channels

    [return: TypeHeuristic(nameof(Channels))]
    IGuildChannelActor Channel(ulong id) => Channels[id];
    EnumerableIndexableGuildChannelLink Channels { get; }

    [return: TypeHeuristic(nameof(TextChannels))]
    ITextChannelActor TextChannel(ulong id) => TextChannels[id];
    EnumerableIndexableTextChannelLink TextChannels { get; }

    [return: TypeHeuristic(nameof(VoiceChannels))]
    IVoiceChannelActor VoiceChannel(ulong id) => VoiceChannels[id];
    EnumerableIndexableVoiceChannelLink VoiceChannels { get; }

    [return: TypeHeuristic(nameof(CategoryChannels))]
    ICategoryChannelActor CategoryChannel(ulong id) => CategoryChannels[id];
    EnumerableIndexableCategoryChannelLink CategoryChannels { get; }

    [return: TypeHeuristic(nameof(AnnouncementChannels))]
    INewsChannelActor AnnouncementChannel(ulong id) => AnnouncementChannels[id];
    EnumerableIndexableNewsChannelLink AnnouncementChannels { get; }

    [return: TypeHeuristic(nameof(ThreadChannels))]
    IThreadChannelActor ThreadChannel(ulong id) => ThreadChannels[id];
    EnumerableIndexableThreadChannelLink ThreadChannels { get; }

    [return: TypeHeuristic(nameof(ActiveThreadChannels))]
    IThreadChannelActor ActiveThreadChannel(ulong id) => ActiveThreadChannels[id];
    EnumerableIndexableThreadChannelLink ActiveThreadChannels { get; }

    [return: TypeHeuristic(nameof(StageChannels))]
    IStageChannelActor StageChannel(ulong id) => StageChannels[id];
    EnumerableIndexableStageChannelLink StageChannels { get; }

    [return: TypeHeuristic(nameof(ForumChannels))]
    IForumChannelActor ForumChannel(ulong id) => ForumChannels[id];
    EnumerableIndexableForumChannelLink ForumChannels { get; }

    [return: TypeHeuristic(nameof(MediaChannels))]
    IMediaChannelActor MediaChannel(ulong id) => MediaChannels[id];
    EnumerableIndexableMediaChannelLink MediaChannels { get; }

    [return: TypeHeuristic(nameof(IntegrationChannels))]
    IIntegrationChannelTrait IntegrationChannel(ulong id) => IntegrationChannels[id];
    EnumerableIndexableIntegrationChannelTraitLink IntegrationChannels { get; }

    #endregion

    [return: TypeHeuristic(nameof(Integrations))]
    IIntegrationActor Integration(ulong id) => Integrations[id];
    EnumerableIndexableIntegrationLink Integrations { get; }

    [return: TypeHeuristic(nameof(Bans))]
    IBanActor Ban(ulong userId) => Bans[userId];
    PagedIndexableBanLink Bans { get; }

    [return: TypeHeuristic(nameof(Members))]
    IMemberActor Member(ulong id) => Members[id];
    PagedIndexableMemberLink Members { get; }

    [return: TypeHeuristic(nameof(Emotes))]
    IGuildEmoteActor Emote(ulong id) => Emotes[id];
    EnumerableIndexableGuildEmoteLink Emotes { get; }

    [return: TypeHeuristic(nameof(Roles))]
    IRoleActor Role(ulong id) => Roles[id];
    EnumerableIndexableRoleLink Roles { get; }

    [return: TypeHeuristic(nameof(Stickers))]
    IGuildStickerActor Sticker(ulong id) => Stickers[id];
    EnumerableIndexableGuildStickerLink Stickers { get; }

    [return: TypeHeuristic(nameof(ScheduledEvents))]
    IGuildScheduledEventActor ScheduledEvent(ulong id) => ScheduledEvents[id];
    EnumerableIndexableGuildScheduledEventLink ScheduledEvents { get; }

    [return: TypeHeuristic(nameof(Invites))]
    IGuildInviteActor Invite(string code) => Invites[code];
    EnumerableIndexableGuildInviteLink Invites { get; }

    [return: TypeHeuristic(nameof(Webhooks))]
    IWebhookActor Webhook(ulong id) => Webhooks[id];
    EnumerableIndexableWebhookLink Webhooks { get; }

    #region Methods

    [return: TypeHeuristic<IEntityProvider<IGuildChannel, IGuildChannelModel>>(nameof(CreateEntity))]
    async Task<IGuildChannel> CreateChannelAsync(
        CreateGuildChannelProperties args, RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateGuildChannel(
                Id,
                args.ToApiModel()
            ),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return CreateEntity(model);
    }

    Task ModifyChannelPositionsAsync(
        IEnumerable<ModifyGuildChannelPositionProperties> channels,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.ModifyGuildChannelPositions(
            Id,
            channels.Select(x => x.ToApiModel()).ToArray()
        ),
        options ?? Client.DefaultRequestOptions,
        token
    );

    [return: TypeHeuristic<IEntityProvider<IMember, IMemberModel>>(nameof(CreateEntity))]
    async Task<IReadOnlyCollection<IMember>> SearchGuildMembersAsync(
        string query,
        int limit = 1000,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.SearchGuildMembers(Id, query, limit),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result.Select(CreateEntity).ToImmutableArray();
    }

    [return: TypeHeuristic<IEntityProvider<IMember, IMemberModel>>(nameof(CreateEntity))]
    async Task<IMember> AddGuildMemberAsync(
        EntityOrId<ulong, IUser> user,
        string accessToken,
        string? nickname = null,
        IEnumerable<EntityOrId<ulong, IRole>>? roles = null,
        bool? mute = null,
        bool? deaf = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.AddGuildMember(Id, user.Id,
                new AddGuildMemberParams
                {
                    AccessToken = accessToken,
                    Nickname = Optional.FromNullable(nickname),
                    IsDeaf = Optional.FromNullable(deaf),
                    IsMute = Optional.FromNullable(mute),
                    RoleIds = Optional.FromNullable(roles).Map(v => v.Select(v => v.Id).ToArray())
                }),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return CreateEntity(result);
    }

    Task AddGuildMemberRoleAsync(
        EntityOrId<ulong, IUser> user,
        EntityOrId<ulong, IRole> role,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.AddGuildMemberRole(
            Id,
            user.Id,
            role.Id
        ),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task RemoveGuildMemberRole(
        EntityOrId<ulong, IUser> user,
        EntityOrId<ulong, IRole> role,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.RemoveGuildMemberRole(
            Id,
            user.Id,
            role.Id
        ),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task RemoveGuildMember(
        EntityOrId<ulong, IUser> user,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.RemoveGuildMember(Id, user.Id),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task CreateGuildBanAsync(
        EntityOrId<ulong, IUser> user,
        int? purgeMessageSeconds = null,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.CreateGuildBan(Id, user.Id,
            new CreateGuildBanParams {DeleteMessageSeconds = Optional.FromNullable(purgeMessageSeconds)}),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task RemoveGuildBanAsync(
        EntityOrId<ulong, IUser> user,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.RemoveGuildBan(Id, user.Id),
        options ?? Client.DefaultRequestOptions,
        token
    );

    async Task<BulkBanResult> BulkGuildBanAsync(
        IEnumerable<EntityOrId<ulong, IUser>> users,
        int? purgeMessageSeconds = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.BulkGuildBan(Id,
                new BulkBanUsersParams
                {
                    UserIds = users.Select(x => x.Id).ToArray(),
                    DeleteMessageSeconds = Optional.FromNullable(purgeMessageSeconds)
                }),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return BulkBanResult.Construct(Client, result);
    }

    async Task<MfaLevel> ModifyGuildMFALevelAsync(
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

    async Task<int> GetGuildPruneCountAsync(
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

    async Task<int?> BeginGuildPruneAsync(
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

    Task DeleteGuildIntegration(
        EntityOrId<ulong, IIntegration> integration,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.DeleteGuildIntegration(Id, integration.Id),
        options ?? Client.DefaultRequestOptions,
        token
    );

    #endregion
}
