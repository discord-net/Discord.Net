namespace Discord.EntityRelationships;

public interface IGuildRelationship : IGuildRelationship<IGuild>;
public interface IGuildRelationship<TGuild> : IRelationship<ulong, TGuild, IGuildEntitySource<TGuild>>
    where TGuild : IGuild
{
    IGuildEntitySource<TGuild> Guild { get; }

    IGuildEntitySource<TGuild> IRelationship<ulong, TGuild, IGuildEntitySource<TGuild>>.RelationshipLoadable => Guild;
}
