using Discord.Models;
using Discord.Rest;

namespace Discord;

[
    Loadable<Routes.GetStageInstance>,
    Modifiable<Routes.UpdateStageInstance, ModifyStageInstanceProperties>,
    Deletable<Routes.DeleteStageInstance>,
    Creatable<Routes.CreateStageInstance, CreateStageInstanceProperties>
    (
        WhenBackLinkingFrom = [typeof(IStageChannelActor)]
    ),
    Refreshable
]
public partial interface IStageInstanceActor :
    IStageChannelActor.Relationship,
    IActor<ulong, IStageInstance>;