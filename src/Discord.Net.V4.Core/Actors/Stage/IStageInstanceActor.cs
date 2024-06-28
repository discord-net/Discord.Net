using Discord.Models;
using Discord.Rest;

namespace Discord.Stage;

using IModifiable =
    IModifiable<ulong, IStageInstanceActor, ModifyStageInstanceProperties, ModifyStageInstanceParams, IStageInstance,
        IStageInstanceModel>;

public interface ILoadableStageInstanceActor :
    IStageInstanceActor,
    ILoadableEntity<ulong, IStageInstance>;

public interface IStageInstanceActor :
    IStageChannelRelationship,
    IModifiable,
    IDeletable<ulong, IStageInstanceActor>,
    IActor<ulong, IStageInstance>
{
    static IApiRoute IDeletable<ulong, IStageInstanceActor>.DeleteRoute(IPathable path,
        ulong id)
        => Routes.DeleteStageInstance(path.Require<IChannel>());

    static IApiInOutRoute<ModifyStageInstanceParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyStageInstanceParams args
    ) => Routes.ModifyStageInstance(path.Require<IChannel>(), args);
}
