namespace Discord;

public partial interface IPollAnswerActor :
    IActor<int, IPollAnswer>,
    IPollActor.CanonicalRelationship
{
    IUserActor.Paged<PagePollVotersParams> Voters { get; }
}