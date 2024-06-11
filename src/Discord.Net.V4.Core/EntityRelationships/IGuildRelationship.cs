namespace Discord.EntityRelationships;

public interface IGuildRelationship : IGuildRelationship<IGuild>;
public interface IGuildRelationship<TGuild> : IRelationship<ulong, TGuild, ILoadableGuildEntitySource<TGuild>>
    where TGuild : class, IGuild
{
    ILoadableGuildEntitySource<TGuild> Guild { get; }

    ILoadableGuildEntitySource<TGuild> IRelationship<ulong, TGuild, ILoadableGuildEntitySource<TGuild>>.RelationshipLoadable => Guild;
}
