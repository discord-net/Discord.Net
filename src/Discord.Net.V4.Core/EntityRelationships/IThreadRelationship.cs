namespace Discord;

public interface IThreadRelationship :
    IRelationship<ulong, IThreadChannel, ILoadableThreadActor>
{
    ILoadableThreadActor Thread { get; }

    ILoadableThreadActor IRelationship<ulong, IThreadChannel, ILoadableThreadActor>.RelationshipLoadable => Thread;
}
