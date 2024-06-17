using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableRoleActor<TRole> :
    IRoleActor,
    ILoadableEntity<ulong, TRole>
    where TRole : class, IRole;

public interface IRoleActor :
    IGuildRelationship,
    IModifiable<ulong, IRoleActor, ModifyRoleProperties, ModifyGuildRoleParams>,
    IDeletable<ulong, IRoleActor>,
    IActor<ulong, IRole>
{
    static BasicApiRoute IDeletable<ulong, IRoleActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildRole(path.Require<IGuild>(), id);

    static ApiBodyRoute<ModifyGuildRoleParams> IModifiable<ulong, IRoleActor, ModifyRoleProperties, ModifyGuildRoleParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildRoleParams args)
        => Routes.ModifyGuildRole(path.Require<IGuild>(), id, args);
}
