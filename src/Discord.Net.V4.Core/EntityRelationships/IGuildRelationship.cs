namespace Discord;

public interface IGuildRelationship : IGuildRelationship<IGuild>;
public interface IGuildRelationship<TGuild> :
    IRelationship<ulong, TGuild, ILoadableGuildActor<TGuild>>
    where TGuild : class, IGuild
{
    ILoadableGuildActor<TGuild> Guild { get; }

    ILoadableGuildActor<TGuild> IRelationship<ulong, TGuild, ILoadableGuildActor<TGuild>>.RelationshipLoadable => Guild;
}
