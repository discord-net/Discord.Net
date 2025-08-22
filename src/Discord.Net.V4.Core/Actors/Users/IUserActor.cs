namespace Discord.Models;

public interface IUserActor :
    IActor<Snowflake, IUser>,
    ILoadable<IUser>
{
    
}