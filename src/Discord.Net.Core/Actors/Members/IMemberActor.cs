using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IMemberActor :
    IActor<Snowflake, IMember>,
    IUserActor,
    IModifiable<IModifyMemberParams, IMember>,
    IDeletable
{
    IMemberRolesLink Roles { get; }
}