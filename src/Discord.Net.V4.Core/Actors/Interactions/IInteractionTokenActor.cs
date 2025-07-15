namespace Discord;

public partial interface IInteractionTokenActor :
    IActor<ulong, IInteraction>
{
    string Token { get; }
    
    IInteractionMessageActor Response { get; }
    
    IInteractionMessageActor.Indexable FollowUps { get; }
}