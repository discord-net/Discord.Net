namespace Discord;

public interface IStageChannelRelationship :
    IRelationship<ulong, IStageChannel, ILoadableStageChannelActor>
{
    ILoadableStageChannelActor Channel { get; }

    ILoadableStageChannelActor IRelationship<ulong, IStageChannel, ILoadableStageChannelActor>.RelationshipLoadable =>
        Channel;
}
