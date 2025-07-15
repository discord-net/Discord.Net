namespace Discord;

// for '/webhooks/{application.id}'
public partial interface IApplicationInteractionActor :
    IActor<ulong, IInteraction>
{
    WithToken this[string token] { get; }

    public partial interface WithToken :
        IApplicationInteractionActor,
        IInteractionTokenActor
    {
        new IInteractionMessageActor.BackLink<WithToken> Response { get; }
        
        new IInteractionMessageActor.Indexable.BackLink<WithToken> FollowUps { get; }

        IInteractionMessageActor IInteractionTokenActor.Response => Response;
        IInteractionMessageActor.Indexable IInteractionTokenActor.FollowUps => FollowUps;

    }
}