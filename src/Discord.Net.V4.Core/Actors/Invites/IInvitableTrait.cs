using Discord.Models;

namespace Discord;

public partial interface IInvitableTrait<out TInviteActor, out TInvite> : IInvitableTrait
    where TInvite : class, IInvite
    where TInviteActor : class, IInviteActor, IActor<string, TInvite>
{
    [SourceOfTruth]
    [return: TypeHeuristic(nameof(Invites))]
    new TInviteActor Invite(string code) => Invites[code];
    
    [SourceOfTruth]
    new ILinkType<TInviteActor, string, TInvite, IInviteModel>.Enumerable.Indexable Invites { get; }
}

public interface IInvitableTrait
{
    [return: TypeHeuristic(nameof(Invites))]
    IInviteActor Invite(string code) => Invites[code];
    InviteLinkType.Enumerable.Indexable Invites { get; }
}