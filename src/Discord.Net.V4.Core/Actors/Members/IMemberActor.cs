using Discord.Models.Models;

namespace Discord.Models;

public interface IMemberActor :
    IActor<Snowflake, IMember>,
    IUserActor,
    IModifiable<IModifyMemberParams, IMember>,
    IDeletable
{
    IMemberRolesLink Roles { get; }
}