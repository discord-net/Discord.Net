namespace Discord;

public interface IThreadableRelationship :
    IRelationship<ulong, IThreadableChannel, ILoadableThreadableChannelActor>
{
    ILoadableThreadableChannelActor Parent { get; }

    ILoadableThreadableChannelActor IRelationship<ulong, IThreadableChannel, ILoadableThreadableChannelActor>.
        RelationshipLoadable => Parent;
}
