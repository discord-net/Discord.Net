using Discord.Stage;

namespace Discord;

public interface IStageInstanceRelationship :
    IRelationship<ulong, IStageInstance, ILoadableStageInstanceActor>
{
    ILoadableStageInstanceActor StageInstance { get; }

    ILoadableStageInstanceActor IRelationship<ulong, IStageInstance, ILoadableStageInstanceActor>.RelationshipLoadable
        => StageInstance;
}
