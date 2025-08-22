using Discord.Models;

namespace Discord;

public interface IMemberRolesLink :
    IIndexableLink<Snowflake, IMemberRoleActor>
{
    Task AddAsync(IdOrEntity<Snowflake, IRole> role, RequestOptions options = default);
}