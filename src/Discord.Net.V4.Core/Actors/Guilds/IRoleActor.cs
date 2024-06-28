using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable = IModifiable<ulong, IRoleActor, ModifyRoleProperties, ModifyGuildRoleParams, IRole, IRoleModel>;

public interface IRoleActor :
    IGuildRelationship,
    IModifiable,
    IDeletable<ulong, IRoleActor>,
    IActor<ulong, IRole>
{
    static IApiRoute IDeletable<ulong, IRoleActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildRole(path.Require<IGuild>(), id);

    static IApiInOutRoute<ModifyGuildRoleParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildRoleParams args
    ) => Routes.ModifyGuildRole(path.Require<IGuild>(), id, args);
}
