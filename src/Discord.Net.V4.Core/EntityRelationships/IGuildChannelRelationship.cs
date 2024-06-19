namespace Discord;

public interface IGuildChannelRelationship :
    IGuildChannelRelationship<ILoadableGuildChannelActor>,
    IGuildRelationship;

public interface IGuildChannelRelationship<out TLoadable> :
    IRelationship<ulong, IGuildChannel, TLoadable>
    where TLoadable : ILoadableGuildChannelActor
{
    TLoadable Channel { get; }

    TLoadable IRelationship<ulong, IGuildChannel, TLoadable>.RelationshipLoadable => Channel;
}
