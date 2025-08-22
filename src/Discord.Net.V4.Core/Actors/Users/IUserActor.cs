using Discord.Models;

namespace Discord;

public interface IUserActor :
    IActor<Snowflake, IUser>,
    ILoadable<IUser>
{
    
}