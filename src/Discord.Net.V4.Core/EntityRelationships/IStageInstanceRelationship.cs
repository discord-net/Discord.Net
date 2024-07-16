using Discord.Stage;

namespace Discord;

public interface IStageInstanceRelationship :
    IRelationship<IStageInstanceActor, ulong, IStageInstance>
{
    IStageInstanceActor StageInstance { get; }

    IStageInstanceActor IRelationship<IStageInstanceActor, ulong, IStageInstance>.RelationshipActor
        => StageInstance;
}
