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
    IEntityProvider<IGuildMember, IMemberModel>
{
    #region Channels

    [return: TypeHeuristic(nameof(Channels))]
    IGuildChannelActor Channel(ulong id) => Channels[id];
    IEnumerableIndexableActor<IGuildChannelActor, ulong, IGuildChannel> Channels { get; }

    [return: TypeHeuristic(nameof(TextChannels))]
    ITextChannelActor TextChannel(ulong id) => TextChannels[id];
    IEnumerableIndexableActor<ITextChannelActor, ulong, ITextChannel> TextChannels { get; }

    [return: TypeHeuristic(nameof(VoiceChannels))]
    IVoiceChannelActor VoiceChannel(ulong id) => VoiceChannels[id];
    IEnumerableIndexableActor<IVoiceChannelActor, ulong, IVoiceChannel> VoiceChannels { get; }

    [return: TypeHeuristic(nameof(CategoryChannels))]
    ICategoryChannelActor CategoryChannel(ulong id) => CategoryChannels[id];
    IEnumerableIndexableActor<ICategoryChannelActor, ulong, ICategoryChannel> CategoryChannels { get; }

    [return: TypeHeuristic(nameof(AnnouncementChannels))]
    INewsChannelActor AnnouncementChannel(ulong id) => AnnouncementChannels[id];
    IEnumerableIndexableActor<INewsChannelActor, ulong, INewsChannel> AnnouncementChannels { get; }

    [return: TypeHeuristic(nameof(ThreadChannels))]
    IThreadChannelActor ThreadChannel(ulong id) => ThreadChannels[id];
    IEnumerableIndexableActor<IThreadChannelActor, ulong, IThreadChannel> ThreadChannels { get; }

    [return: TypeHeuristic(nameof(Invites))]
    IThreadChannelActor ActiveThreadChannel(ulong id) => ActiveThreadChannels[id];
    IEnumerableIndexableActor<IThreadChannelActor, ulong, IThreadChannel> ActiveThreadChannels { get; }

    [return: TypeHeuristic(nameof(StageChannels))]
    IStageChannelActor StageChannel(ulong id) => StageChannels[id];
    IEnumerableIndexableActor<IStageChannelActor, ulong, IStageChannel> StageChannels { get; }

    [return: TypeHeuristic(nameof(ForumChannels))]
    IForumChannelActor ForumChannel(ulong id) => ForumChannels[id];
    IEnumerableIndexableActor<IForumChannelActor, ulong, IForumChannel> ForumChannels { get; }

    [return: TypeHeuristic(nameof(MediaChannels))]
    IMediaChannelActor MediaChannel(ulong id) => MediaChannels[id];
    IEnumerableIndexableActor<IMediaChannelActor, ulong, IMediaChannel> MediaChannels { get; }

    #endregion

    [return: TypeHeuristic(nameof(Integrations))]
    IIntegrationActor Integration(ulong id) => Integrations[id];
    IEnumerableIndexableActor<IIntegrationActor, ulong, IIntegration> Integrations { get; }

    [return: TypeHeuristic(nameof(Bans))]
    IBanActor Ban(ulong userId) => Bans[userId];
    IPagedIndexableActor<IBanActor, ulong, IBan, PageGuildBansParams> Bans { get; }

    [return: TypeHeuristic(nameof(Members))]
    IGuildMemberActor Member(ulong id) => Members[id];
    IPagedIndexableActor<IGuildMemberActor, ulong, IGuildMember, PageGuildMembersParams> Members { get; }

    [return: TypeHeuristic(nameof(Emotes))]
    IGuildEmoteActor Emote(ulong id) => Emotes[id];
    IEnumerableIndexableActor<IGuildEmoteActor, ulong, IGuildEmote> Emotes { get; }

    [return: TypeHeuristic(nameof(Roles))]
    IRoleActor Role(ulong id) => Roles[id];
    IEnumerableIndexableActor<IRoleActor, ulong, IRole> Roles { get; }

    [return: TypeHeuristic(nameof(Stickers))]
    IGuildStickerActor Sticker(ulong id) => Stickers[id];
    IEnumerableIndexableActor<IGuildStickerActor, ulong, IGuildSticker> Stickers { get; }

    [return: TypeHeuristic(nameof(ScheduledEvents))]
    IGuildScheduledEventActor ScheduledEvent(ulong id) => ScheduledEvents[id];
    IEnumerableIndexableActor<IGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents { get; }

    [return: TypeHeuristic(nameof(Invites))]
    IInviteActor Invite(string code) => Invites[code];
    IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites { get; }

    [return: TypeHeuristic(nameof(Webhooks))]
    IWebhookActor Webhook(ulong id) => Webhooks[id];
    IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks { get; }

    #region Methods

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

    async Task<IReadOnlyCollection<IGuildMember>> SearchGuildMembersAsync(
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

    async Task<IGuildMember> AddGuildMemberAsync(
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
