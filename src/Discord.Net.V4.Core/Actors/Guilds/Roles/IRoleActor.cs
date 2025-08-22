using Discord.Models.Models;

namespace Discord.Models;

public interface IRoleActor :
    IActor<Snowflake, IRole>,
    IModifiable<IModifyRoleParams, IRole>,
    IDeletable
{
    
}