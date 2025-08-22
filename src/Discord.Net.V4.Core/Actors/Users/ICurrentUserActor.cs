namespace Discord.Models;

public interface ICurrentUserActor :
    IActor<Snowflake, ICurrentUser>,
    IUserActor
{
}