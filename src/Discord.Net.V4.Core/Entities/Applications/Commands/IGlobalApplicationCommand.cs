using Discord.Rest;

namespace Discord;

[
    Refreshable(nameof(Routes.GetGlobalApplicationCommand)),
    FetchableOfMany(nameof(Routes.GetGlobalApplicationCommands))
]
public partial interface IGlobalApplicationCommand :
    IApplicationCommand,
    IGlobalApplicationCommandActor;