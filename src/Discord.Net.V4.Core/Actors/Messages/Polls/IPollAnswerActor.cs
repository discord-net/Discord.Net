namespace Discord;

public partial interface IPollAnswerActor :
    IActor<int, IPollAnswer>,
    IPollRelationship,
    IMessageRelationship,
    IChannelRelationship<IMessageChannelTrait, IMessageChannel>
{
    IUserActor.Paged<PagePollVotersParams> Voters { get; }
}