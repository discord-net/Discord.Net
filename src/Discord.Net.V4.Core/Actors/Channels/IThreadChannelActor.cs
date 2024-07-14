using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableThreadChannelActor :
    IThreadChannelActor,
    ILoadableEntity<ulong, IThreadChannel>;

[Modifiable<ModifyThreadChannelProperties>(nameof(Routes.ModifyChannel))]
public partial interface IThreadChannelActor :
    IGuildChannelActor,
    IMessageChannelActor,
    IThreadMemberRelationship,
    IActor<ulong, IThreadChannel>
{
    ILoadableThreadMemberActor CurrentThreadMember { get; }

    IEnumerableIndexableActor<ILoadableThreadMemberActor, ulong, IThreadMember> ThreadMembers { get; }

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
