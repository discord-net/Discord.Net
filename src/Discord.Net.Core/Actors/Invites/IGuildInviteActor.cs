namespace Discord;

public interface IGuildInviteActor :
    IActor<InviteId, IGuildInvite>,
    IInviteActor
{
    IGuildActor Guild { get; }
}