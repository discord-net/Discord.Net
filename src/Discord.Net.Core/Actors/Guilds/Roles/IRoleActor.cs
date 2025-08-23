using Discord.Models;

namespace Discord;

public interface IRoleActor :
    IActor<Snowflake, IRole>,
    IModifiable<IModifyRoleParams, IRole>,
    IDeletable
{
    
}