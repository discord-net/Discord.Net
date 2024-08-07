namespace Discord;

public interface IMemberRelationship :
    IRelationship<IMemberActor, ulong, IMember>
{
    IMemberActor Member { get; }

    IMemberActor IRelationship<IMemberActor, ulong, IMember>.RelationshipActor
        => Member;
}
