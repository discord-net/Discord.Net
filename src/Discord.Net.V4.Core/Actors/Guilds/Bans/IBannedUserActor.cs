using Discord.Models;

namespace Discord;

public interface IBannedUserActor :
    IActor<Snowflake, IBannedUser>,
    IUserActor,
    IDeletable
{
    
}