using Discord.Models;
using Discord.Rest;

namespace Discord.Stage;

public interface ILoadableStageInstanceActor :
    IStageInstanceActor,
    ILoadableEntity<ulong, IStageInstance>;

public interface IStageInstanceActor :
    IStageChannelRelationship,
    IModifiable<ulong, IStageInstanceActor, ModifyStageInstanceProperties, ModifyStageInstanceParams>,
    IDeletable<ulong, IStageInstanceActor>,
    IActor<ulong, IStageInstance>
{
    static IApiRoute IDeletable<ulong, IStageInstanceActor>.DeleteRoute(IPathable path,
        ulong id)
        => Routes.DeleteStageInstance(path.Require<IChannel>());

    static IApiInRoute<ModifyStageInstanceParams>
        IModifiable<ulong, IStageInstanceActor, ModifyStageInstanceProperties,
            ModifyStageInstanceParams>.ModifyRoute(IPathable path, ulong id, ModifyStageInstanceParams args)
        => Routes.ModifyStageInstance(path.Require<IChannel>(), args);
}
