using Discord.Rest;

namespace Discord;

public interface ILoadableThreadMemberActor<TThreadMember> :
    IThreadMemberActor<TThreadMember>,
    ILoadableEntity<ulong, TThreadMember>
    where TThreadMember : class, IThreadMember;

public interface IThreadMemberActor<out TThreadMember> :
    IActor<ulong, TThreadMember>,
    IThreadRelationship,
    IMemberRelationship,
    IUserRelationship
    where TThreadMember : class, IThreadMember
{
    Task RemoveAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(
            Routes.RemoveThreadMember(Thread.Id, Id),
            options ?? Client.DefaultRequestOptions, token
        );
}
