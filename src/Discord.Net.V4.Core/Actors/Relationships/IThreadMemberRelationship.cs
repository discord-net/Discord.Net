namespace Discord;

public interface IThreadMemberRelationship :
    IRelationship<IThreadMemberActor, ulong, IThreadMember>
{
    IThreadMemberActor ThreadMember { get; }

    IThreadMemberActor IRelationship<IThreadMemberActor, ulong, IThreadMember>.RelationshipActor
        => ThreadMember;
}
