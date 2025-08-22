namespace Discord.Models;

public interface IBannedUserActor :
    IActor<Snowflake, IBannedUser>,
    IUserActor,
    IDeletable
{
    
}