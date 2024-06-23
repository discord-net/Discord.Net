using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableThreadActor :
    IThreadActor,
    ILoadableEntity<ulong, IThreadChannel>;

public interface IThreadActor :
    IGuildChannelActor,
    IMessageChannelActor,
    IThreadMemberRelationship,
    IActor<ulong, IThreadChannel>,
    IModifiable<ulong, IThreadActor, ModifyThreadChannelProperties, ModifyThreadChannelParams>
{
    ILoadableThreadMemberActor CurrentThreadMember { get; }

    IEnumerableIndexableActor<ILoadableThreadMemberActor, ulong, IThreadMember> ThreadMembers { get; }

    static IApiInRoute<ModifyThreadChannelParams>
        IModifiable<ulong, IThreadActor, ModifyThreadChannelProperties, ModifyThreadChannelParams>.ModifyRoute(
            IPathable path, ulong id,
            ModifyThreadChannelParams args)
        => Routes.ModifyChannel(id, args);

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
