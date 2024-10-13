using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetApplicationCommandPermissions)),
    Modifiable<ModifyApplicationCommandPermissionsProperties>(nameof(Routes.ModifyApplicationCommandPermissions))
]
public partial interface IGuildApplicationCommandPermissionsActor :
    IActor<ulong, IGuildApplicationCommandPermissionses>,
    IGuildApplicationCommandActor.CanonicalRelationship
{
    
}