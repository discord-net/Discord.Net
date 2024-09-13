using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetGlobalApplicationCommand)),
    Deletable(nameof(Routes.DeleteGlobalApplicationCommand)),
    Modifiable<ModifyGlobalApplicationCommandProperties>(nameof(Routes.ModifyGlobalApplicationCommand)),
    Creatable<CreateGlobalApplicationCommandProperties>(nameof(Routes.CreateGlobalApplicationCommand))
]
public partial interface IGlobalApplicationCommandActor :
    IApplicationCommandActor,
    IActor<ulong, IGlobalApplicationCommand>
{
    
}