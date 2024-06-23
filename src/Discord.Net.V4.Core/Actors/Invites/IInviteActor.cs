using Discord.Rest;

namespace Discord;

public interface ILoadableInviteActor<TInvite> :
    IInviteActor,
    ILoadableEntity<string, TInvite>
    where TInvite : class, IInvite;

public interface IInviteActor :
    IDeletable<string, IInviteActor>,
    IActor<string, IInvite>
{
    static IApiRoute IDeletable<string, IInviteActor>.DeleteRoute(IPathable path, string id)
        => Routes.DeleteInvite(id);
}
