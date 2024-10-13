using Discord.Models;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetStageInstance))]
[Modifiable<ModifyStageInstanceProperties>(nameof(Routes.ModifyStageInstance))]
[Deletable(nameof(Routes.DeleteStageInstance))]
[Creatable<CreateStageInstanceProperties>(nameof(Routes.CreateStageInstance))]
public partial interface IStageInstanceActor :
    IStageChannelActor.Relationship,
    IActor<ulong, IStageInstance>;
