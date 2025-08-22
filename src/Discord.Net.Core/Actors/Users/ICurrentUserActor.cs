using Discord.Models;

namespace Discord;

public interface ICurrentUserActor :
    IActor<Snowflake, ICurrentUser>,
    IUserActor
{
}