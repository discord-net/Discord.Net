namespace Discord.Models;

public interface IMemberRolesLink :
    IIndexableLink<Snowflake, IMemberRoleActor>
{
    Task AddAsync(IdOrEntity<Snowflake, IRole> role, RequestOptions options = default);
}