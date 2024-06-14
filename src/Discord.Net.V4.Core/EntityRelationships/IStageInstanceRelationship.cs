using Discord.Stage;

namespace Discord;

public interface IStageInstanceRelationship : IStageInstanceRelationship<IStageInstance>;

public interface IStageInstanceRelationship<TStageInstance> :
    IRelationship<ulong, TStageInstance, ILoadableStageInstanceActor<TStageInstance>>
    where TStageInstance : class, IStageInstance<TStageInstance>
{
    ILoadableStageInstanceActor<TStageInstance> StageInstance { get; }

    ILoadableStageInstanceActor<TStageInstance> IRelationship<ulong, TStageInstance, ILoadableStageInstanceActor<TStageInstance>>.RelationshipLoadable
        => StageInstance;
}
