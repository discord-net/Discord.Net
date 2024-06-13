using Discord.Stage;

namespace Discord;

using Relationship = IRelationship<ulong, IStageInstance, ILoadableStageInstanceActor<IStageInstance>>;

public interface IStageInstanceRelationship
    : Relationship
{
    ILoadableStageInstanceActor<IStageInstance> StageInstance { get; }

    ILoadableStageInstanceActor<IStageInstance> Relationship.RelationshipLoadable
        => StageInstance;
}
