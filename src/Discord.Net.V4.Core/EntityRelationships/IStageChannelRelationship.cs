namespace Discord;

public interface IStageChannelRelationship : IStageChannelRelationship<IStageChannel>;

public interface IStageChannelRelationship<TStageChannel> :
    IGuildChannelRelationship<TStageChannel, ILoadableStageChannelActor<TStageChannel>>
    where TStageChannel : class, IStageChannel;
