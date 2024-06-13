using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableRoleActor<TRole> :
    IRoleActor<TRole>,
    ILoadableEntity<ulong, TRole>
    where TRole : class, IRole;

public interface IRoleActor<out TRole> :
    IGuildRelationship,
    IModifiable<ulong, IRoleActor<TRole>, ModifyRoleProperties, ModifyGuildRoleParams>,
    IDeletable<ulong, IRoleActor<TRole>>,
    IActor<ulong, TRole>
    where TRole : IRole
{
    static BasicApiRoute IDeletable<ulong, IRoleActor<TRole>>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildRole(path.Require<IGuild>(), id);

    static ApiBodyRoute<ModifyGuildRoleParams> IModifiable<ulong, IRoleActor<TRole>, ModifyRoleProperties, ModifyGuildRoleParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildRoleParams args)
        => Routes.ModifyGuildRole(path.Require<IGuild>(), id, args);
}
