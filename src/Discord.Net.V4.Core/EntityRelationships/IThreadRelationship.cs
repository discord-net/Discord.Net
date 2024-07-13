namespace Discord;

public interface IThreadRelationship :
    IRelationship<ulong, IThreadChannel, ILoadableThreadChannelActor>
{
    ILoadableThreadChannelActor Thread { get; }

    ILoadableThreadChannelActor IRelationship<ulong, IThreadChannel, ILoadableThreadChannelActor>.RelationshipLoadable => Thread;
}
