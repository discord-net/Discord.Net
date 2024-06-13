namespace Discord;

public interface IThreadMemberRelationship : IThreadMemberRelationship<IThreadMember>;
public interface IThreadMemberRelationship<TTheadMember> :
    IRelationship<ulong, TTheadMember, ILoadableThreadMemberActor<TTheadMember>>
    where TTheadMember : class, IThreadMember
{
    ILoadableThreadMemberActor<TTheadMember> ThreadMember { get; }

    ILoadableThreadMemberActor<TTheadMember>
        IRelationship<ulong, TTheadMember, ILoadableThreadMemberActor<TTheadMember>>.RelationshipLoadable =>
        ThreadMember;
}
