using Discord.Models;

namespace Discord;

[Trait]
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

[Trait]
public interface IInvitableTrait
{
    [return: TypeHeuristic(nameof(Invites))]
    IInviteActor Invite(string code) => Invites[code];
    InviteLink.Enumerable.Indexable Invites { get; }
}