using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord;

public interface ILoadableGuildActor :
    IGuildActor,
    ILoadableEntity<ulong, IGuild>;

[Modifiable<ModifyGuildProperties>(nameof(Routes.ModifyGuild))]
[Deletable(nameof(Routes.DeleteGuild))]
public partial interface IGuildActor :
    IActor<ulong, IGuild>
{
    #region Channels

    IEnumerableIndexableActor<ILoadableGuildChannelActor, ulong, IGuildChannel> Channels { get; }
    ILoadableGuildChannelActor Channel(ulong id) => Channels[id];

    IEnumerableIndexableActor<ILoadableTextChannelActor, ulong, ITextChannel> TextChannels { get; }
    ILoadableTextChannelActor TextChannel(ulong id) => TextChannels[id];

    IEnumerableIndexableActor<ILoadableVoiceChannelActor, ulong, IVoiceChannel> VoiceChannels { get; }
    ILoadableVoiceChannelActor VoiceChannel(ulong id) => VoiceChannels[id];

    IEnumerableIndexableActor<ILoadableCategoryChannelActor, ulong, ICategoryChannel> CategoryChannels { get; }
    ILoadableCategoryChannelActor CategoryChannel(ulong id) => CategoryChannels[id];

    IEnumerableIndexableActor<ILoadableNewsChannelActor, ulong, INewsChannel> AnnouncementChannels { get; }
    ILoadableNewsChannelActor AnnouncementChannel(ulong id) => AnnouncementChannels[id];

    IEnumerableIndexableActor<ILoadableThreadChannelActor, ulong, IThreadChannel> ThreadChannels { get; }
    ILoadableThreadChannelActor ThreadChannel(ulong id) => ThreadChannels[id];

    IEnumerableIndexableActor<ILoadableThreadChannelActor, ulong, IThreadChannel> ActiveThreadChannels { get; }
    ILoadableThreadChannelActor ActiveThreadChannel(ulong id) => ActiveThreadChannels[id];

    IEnumerableIndexableActor<ILoadableStageChannelActor, ulong, IStageChannel> StageChannels { get; }
    ILoadableStageChannelActor StageChannel(ulong id) => StageChannels[id];

    IEnumerableIndexableActor<ILoadableForumChannelActor, ulong, IForumChannel> ForumChannels { get; }
    ILoadableForumChannelActor ForumChannel(ulong id) => ForumChannels[id];

    IEnumerableIndexableActor<ILoadableMediaChannelActor, ulong, IMediaChannel> MediaChannels { get; }
    ILoadableMediaChannelActor MediaChannel(ulong id) => MediaChannels[id];

    #endregion

    IEnumerableIndexableActor<IIntegrationActor, ulong, IIntegration> Integrations { get; }
    IIntegrationActor Integration(ulong id) => Integrations[id];

    IPagedIndexableActor<ILoadableBanActor, ulong, IBan, PageGuildBansParams> Bans { get; }
    ILoadableBanActor Ban(ulong userId) => Bans[userId];

    IPagedIndexableActor<ILoadableGuildMemberActor, ulong, IGuildMember, PageGuildMembersParams> Members { get; }
    ILoadableGuildMemberActor Member(ulong id) => Members[id];

    IEnumerableIndexableActor<ILoadableGuildEmoteActor, ulong, IGuildEmote> Emotes { get; }
    ILoadableGuildEmoteActor Emote(ulong id) => Emotes[id];

    IEnumerableIndexableActor<IRoleActor, ulong, IRole> Roles { get; }
    IRoleActor Role(ulong id) => Roles[id];

    IEnumerableIndexableActor<ILoadableGuildStickerActor, ulong, IGuildSticker> Stickers { get; }
    ILoadableGuildStickerActor Sticker(ulong id) => Stickers[id];

    IEnumerableIndexableActor<ILoadableGuildScheduledEventActor, ulong, IGuildScheduledEvent> ScheduledEvents { get; }
    ILoadableGuildScheduledEventActor ScheduledEvent(ulong id) => ScheduledEvents[id];

    IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites { get; }
    ILoadableInviteActor<IInvite> Invite(string code) => Invites[code];

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

        return Client.CreateEntity(model);
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

        return result.Select(Client.CreateEntity).ToImmutableArray();
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

        return Client.CreateEntity(result);
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
                includeRoles.Map(v => v.Select(v => v.Id).ToArray())
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
