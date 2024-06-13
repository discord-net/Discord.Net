using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableThreadActor<TThread> :
    IThreadActor<TThread>,
    ILoadableGuildChannelActor<TThread>
    where TThread : class, IThreadChannel<TThread>;

public interface IThreadActor<out TThread> :
    IGuildChannelActor<TThread>,
    IThreadMemberRelationship,
    IModifiable<ulong, IThreadActor<TThread>, ModifyThreadChannelProperties, ModifyThreadChannelParams>
    where TThread : IThreadChannel<TThread>
{
    ILoadableThreadMemberActor<IThreadMember> CurrentThreadMember { get; }

    ILoadableRootActor<ILoadableThreadMemberActor<IThreadMember>, ulong, IThreadMember> ThreadMembers { get; }

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

    ILoadableThreadMemberActor<IThreadMember> IThreadMemberRelationship<IThreadMember>.ThreadMember
        => CurrentThreadMember;

    static ApiBodyRoute<ModifyThreadChannelParams> IModifiable<ulong, IThreadActor<TThread>, ModifyThreadChannelProperties, ModifyThreadChannelParams>.ModifyRoute(IPathable path, ulong id,
        ModifyThreadChannelParams args)
        => Routes.ModifyChannel(id, args);
}
