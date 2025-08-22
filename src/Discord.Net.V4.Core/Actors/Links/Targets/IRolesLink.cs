using Discord.Models.Models;

namespace Discord.Models;

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