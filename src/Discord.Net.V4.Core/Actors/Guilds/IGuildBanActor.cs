using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildBanActor :
    IGuildBanActor,
    ILoadableEntity<ulong, IBan>;

public interface IGuildBanActor :
    IGuildRelationship,
    IUserRelationship,
    IActor<ulong, IBan>,
    IDeletable<ulong, IGuildBanActor>
{
    static IApiRoute IDeletable<ulong, IGuildBanActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.RemoveGuildBan(path.Require<IGuild>(), id);
}
