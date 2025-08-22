using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IRoleActor :
    IActor<Snowflake, IRole>,
    IModifiable<IModifyRoleParams, IRole>,
    IDeletable
{
    
}