using Discord.Integration;
using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

public interface ILoadableGuildActor<TGuild> :
    IGuildActor,
    ILoadableEntity<ulong, TGuild>
    where TGuild : class, IGuild;

public interface IGuildActor :
    IActor<ulong, IGuild>,
    IModifiable<ulong, IGuildActor, ModifyGuildProperties, ModifyGuildParams>,
    IDeletable<ulong, IGuildActor>
{
    #region Sub-actors

    IRootActor<IIntegrationActor, ulong, IIntegration> Integrations { get; }
    IIntegrationActor Integration(ulong id) => Integrations[id];

    IPagedLoadableRootActor<ILoadableGuildBanActor<IBan>, ulong, IBan> Bans { get; }
    ILoadableGuildBanActor<IBan> Ban(ulong userId) => Bans[userId];

    IRootActor<ILoadableStageChannelActor<IStageChannel>, ulong, IStageChannel> StageChannels { get; }
    ILoadableStageChannelActor<IStageChannel> StageChannel(ulong id) => StageChannels[id];

    ILoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> ActiveThreads { get; }
    ILoadableThreadActor<IThreadChannel> ActiveThread(ulong id) => ActiveThreads[id];

    IRootActor<ILoadableTextChannelActor<ITextChannel>, ulong, ITextChannel> TextChannels { get; }
    ILoadableTextChannelActor<ITextChannel> TextChannel(ulong id) => TextChannels[id];

    ILoadableRootActor<ILoadableGuildChannelActor<IGuildChannel>, ulong, IGuildChannel> Channels { get; }
    ILoadableGuildChannelActor<IGuildChannel> Channel(ulong id) => Channels[id];

    IPagedLoadableRootActor<ILoadableGuildMemberActor<IGuildMember>, ulong, IGuildMember> Members { get; }
    ILoadableGuildMemberActor<IGuildMember> Member(ulong id) => Members[id];

    ILoadableRootActor<ILoadableGuildEmoteActor<IGuildEmote>, ulong, IGuildEmote> Emotes { get; }
    ILoadableGuildEmoteActor<IGuildEmote> Emote(ulong id) => Emotes[id];

    ILoadableRootActor<ILoadableRoleActor<IRole>, ulong, IRole> Roles { get; }
    ILoadableRoleActor<IRole> Role(ulong id) => Roles[id];

    ILoadableRootActor<ILoadableGuildStickerActor<IGuildSticker>, ulong, IGuildSticker> Stickers { get; }
    ILoadableGuildStickerActor<IGuildSticker> Sticker(ulong id) => Stickers[id];

    ILoadableRootActor<ILoadableGuildScheduledEventActor<IGuildScheduledEvent>, ulong, IGuildScheduledEvent> ScheduledEvents { get; }
    ILoadableGuildScheduledEventActor<IGuildScheduledEvent> ScheduledEvent(ulong id) => ScheduledEvents[id];

    ILoadableRootActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites { get; }
    ILoadableInviteActor<IInvite> Invite(string code) => Invites[code];

    #endregion

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
            Routes.AddGuildMember(Id, user.Id, new AddGuildMemberParams
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
        Routes.CreateGuildBan(Id, user.Id, new CreateGuildBanParams()
        {
            DeleteMessageSeconds = Optional.FromNullable(purgeMessageSeconds)
        }),
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
            Routes.BulkGuildBan(Id, new BulkBanUsersParams()
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
            Routes.ModifyGuildMfaLevel(Id, new ModifyGuildMfaLevelParams()
            {
                Level = (int)level
            }),
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
                new BeginGuildPruneParams()
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


    static BasicApiRoute IDeletable<ulong, IGuildActor>
        .DeleteRoute(IPathable path, ulong id) => Routes.DeleteGuild(id);

    static ApiBodyRoute<ModifyGuildParams> IModifiable<ulong, IGuildActor, ModifyGuildProperties, ModifyGuildParams>
        .ModifyRoute(IPathable path, ulong id, ModifyGuildParams args) => Routes.ModifyGuild(id, args);
}
