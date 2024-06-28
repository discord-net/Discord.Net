using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IThreadChannelActor, ModifyThreadChannelProperties, ModifyThreadChannelParams, IThreadChannel,
        IThreadChannelModel>;

public interface ILoadableThreadChannelActor :
    IThreadChannelActor,
    ILoadableEntity<ulong, IThreadChannel>;

public interface IThreadChannelActor :
    IGuildChannelActor,
    IMessageChannelActor,
    IThreadMemberRelationship,
    IActor<ulong, IThreadChannel>,
    IModifiable
{
    ILoadableThreadMemberActor CurrentThreadMember { get; }

    IEnumerableIndexableActor<ILoadableThreadMemberActor, ulong, IThreadMember> ThreadMembers { get; }

    static IApiInOutRoute<ModifyThreadChannelParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyThreadChannelParams args
    ) => Routes.ModifyChannel(id, args);

    ILoadableThreadMemberActor IThreadMemberRelationship.ThreadMember
        => CurrentThreadMember;

    Task JoinAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.JoinThread(Id), options ?? Client.DefaultRequestOptions, token);

    Task AddThreadMemberAsync(EntityOrId<ulong, IUser> user, RequestOptions? options = null,
        CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(
            Routes.AddThreadMember(Id, user.Id),
            options ?? Client.DefaultRequestOptions,
            token
        );

    Task LeaveAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.LeaveThread(Id), options ?? Client.DefaultRequestOptions, token);
}
