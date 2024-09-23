namespace Discord;

public interface IPollRelationship : IRelationship<IPollActor, ulong, IPoll>
{
    IPollActor Poll { get; }

    IPollActor IRelationship<IPollActor, ulong, IPoll>.RelationshipActor => Poll;
}