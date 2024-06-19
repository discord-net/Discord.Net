namespace Discord;

public interface IMemberRelationship :
    IRelationship<ulong, IGuildMember, ILoadableGuildMemberActor>
{
    ILoadableGuildMemberActor Member { get; }

    ILoadableGuildMemberActor IRelationship<ulong, IGuildMember, ILoadableGuildMemberActor>.RelationshipLoadable => Member;
}
