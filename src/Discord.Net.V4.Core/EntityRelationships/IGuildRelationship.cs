namespace Discord;

public interface IGuildRelationship :
    IRelationship<IGuildActor, ulong, IGuild>
{
    IGuildActor Guild { get; }

    IGuildActor IRelationship<IGuildActor, ulong, IGuild>.RelationshipActor => Guild;
}
