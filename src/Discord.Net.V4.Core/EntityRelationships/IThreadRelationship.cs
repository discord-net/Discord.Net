namespace Discord;

public interface IThreadRelationship :
    IRelationship<ulong, IThreadChannel, ILoadableThreadChannelActor>
{
    ILoadableThreadChannelActor ThreadChannel { get; }

    ILoadableThreadChannelActor IRelationship<ulong, IThreadChannel, ILoadableThreadChannelActor>.RelationshipLoadable => ThreadChannel;
}
