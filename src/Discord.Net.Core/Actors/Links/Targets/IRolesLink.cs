using Discord.Models;

namespace Discord;

public interface IRolesLink :
    IIndexableLink<Snowflake, IRoleActor>,
    IBatchLink<IRole>,
    ICreatable<ICreateRoleParams, IRole>
{
    Task<IReadOnlyList<IRole>> ModifyPositionsAsync(
        IReadOnlyCollection<IModifyRolePositionParams> positions,
        RequestOptions options = default
    );
}