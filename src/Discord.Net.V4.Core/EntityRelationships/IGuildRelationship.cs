namespace Discord;

public interface IGuildRelationship :
    IRelationship<ulong, IGuild, ILoadableGuildActor>
{
    ILoadableGuildActor Guild { get; }

    ILoadableGuildActor IRelationship<ulong, IGuild, ILoadableGuildActor>.RelationshipLoadable => Guild;
}
