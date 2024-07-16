namespace Discord;

public interface IMemberRelationship :
    IRelationship<IGuildMemberActor, ulong, IGuildMember>
{
    IGuildMemberActor Member { get; }

    IGuildMemberActor IRelationship<IGuildMemberActor, ulong, IGuildMember>.RelationshipActor
        => Member;
}
