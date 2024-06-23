using Discord.Rest;

namespace Discord;

public interface ILoadableThreadMemberActor :
    IThreadMemberActor,
    ILoadableEntity<ulong, IThreadMember>;

public interface IThreadMemberActor :
    IActor<ulong, IThreadMember>,
    IThreadRelationship,
    IMemberRelationship,
    IUserRelationship
{
    Task RemoveAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(
            Routes.RemoveThreadMember(Thread.Id, Id),
            options ?? Client.DefaultRequestOptions, token
        );
}
