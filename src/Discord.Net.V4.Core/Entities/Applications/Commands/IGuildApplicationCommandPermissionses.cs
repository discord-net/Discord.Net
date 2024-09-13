using Discord.Models;

namespace Discord;

public partial interface IGuildApplicationCommandPermissionses :
    ISnowflakeEntity<IGuildApplicationCommandPermissionsModel>,
    IGuildApplicationCommandPermissionsActor
{
    
}