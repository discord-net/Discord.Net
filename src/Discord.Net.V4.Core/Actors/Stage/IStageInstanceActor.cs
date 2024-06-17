using Discord.Models;
using Discord.Rest;

namespace Discord.Stage;

public interface ILoadableStageInstanceActor<TStageInstance> :
    IStageInstanceActor,
    ILoadableEntity<ulong, TStageInstance>
    where TStageInstance : class, IStageInstance<TStageInstance>;

public interface IStageInstanceActor :
    IStageChannelRelationship,
    IModifiable<ulong, IStageInstanceActor, ModifyStageInstanceProperties, ModifyStageInstanceParams>,
    IDeletable<ulong, IStageInstanceActor>,
    IActor<ulong, IStageInstance>
{
    static BasicApiRoute IDeletable<ulong, IStageInstanceActor>.DeleteRoute(IPathable path,
        ulong id)
        => Routes.DeleteStageInstance(path.Require<IChannel>());

    static ApiBodyRoute<ModifyStageInstanceParams>
        IModifiable<ulong, IStageInstanceActor, ModifyStageInstanceProperties,
            ModifyStageInstanceParams>.ModifyRoute(IPathable path, ulong id, ModifyStageInstanceParams args)
        => Routes.ModifyStageInstance(path.Require<IChannel>(), args);
}
