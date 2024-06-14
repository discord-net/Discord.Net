using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord;

public interface ILoadableGuildActor<TGuild> : IGuildActor<TGuild>, ILoadableEntity<ulong, TGuild>
    where TGuild : class, IGuild;

public interface IGuildActor<out TGuild> :
    IActor<ulong, TGuild>,
    IModifiable<ulong, IGuildActor<TGuild>, ModifyGuildProperties, ModifyGuildParams>,
    IDeletable<ulong, IGuildActor<TGuild>>
    where TGuild : IGuild
{
    #region Sub-actors

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



    #endregion


    static BasicApiRoute IDeletable<ulong, IGuildActor<TGuild>>
        .DeleteRoute(IPathable path, ulong id) => Routes.DeleteGuild(id);

    static ApiBodyRoute<ModifyGuildParams> IModifiable<ulong, IGuildActor<TGuild>, ModifyGuildProperties, ModifyGuildParams>
        .ModifyRoute(IPathable path, ulong id, ModifyGuildParams args) => Routes.ModifyGuild(id, args);
}
