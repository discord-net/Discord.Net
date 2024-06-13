using Discord.Models;
using Discord.Rest;

namespace Discord.Stage;

public interface ILoadableStageInstanceActor<TStageInstance> :
    IStageInstanceActor<TStageInstance>,
    ILoadableEntity<ulong, TStageInstance>
    where TStageInstance : class, IStageInstance;

public interface IStageInstanceActor<out TStageInstance> :
    IStageChannelRelationship,
    IModifiable<ulong, IStageInstanceActor<TStageInstance>, ModifyStageInstanceProperties, ModifyStageInstanceParams>,
    IDeletable<ulong, IStageInstanceActor<TStageInstance>>,
    IActor<ulong, TStageInstance>
    where TStageInstance : IStageInstance
{
    static BasicApiRoute IDeletable<ulong, IStageInstanceActor<TStageInstance>>.DeleteRoute(IPathable path,
        ulong id)
        => Routes.DeleteStageInstance(path.Require<IChannel>());

    static ApiBodyRoute<ModifyStageInstanceParams>
        IModifiable<ulong, IStageInstanceActor<TStageInstance>, ModifyStageInstanceProperties,
            ModifyStageInstanceParams>.ModifyRoute(IPathable path, ulong id, ModifyStageInstanceParams args)
        => Routes.ModifyStageInstance(path.Require<IChannel>(), args);
}
