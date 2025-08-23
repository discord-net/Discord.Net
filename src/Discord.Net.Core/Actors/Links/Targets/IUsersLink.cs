using Discord.Models;

namespace Discord;

public interface IUsersLink : IIndexableLink<Snowflake, IUserActor>
{
    ICurrentUserActor Current { get; }
}