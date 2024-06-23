namespace Discord;

public interface IThreadMemberRelationship :
    IRelationship<ulong, IThreadMember, ILoadableThreadMemberActor>
{
    ILoadableThreadMemberActor ThreadMember { get; }

    ILoadableThreadMemberActor
        IRelationship<ulong, IThreadMember, ILoadableThreadMemberActor>.RelationshipLoadable =>
        ThreadMember;
}
