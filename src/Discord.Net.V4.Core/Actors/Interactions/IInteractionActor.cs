using Discord.Models;

namespace Discord;


// for '/interactions/{interaction.id}'
public partial interface IInteractionActor :
    IApplicationInteractionActor,
    IEntityProvider<IInteractionCallbackResponse, IInteractionCallbackResponseModel>
{
    new WithToken this[string token] { get; }

    public new partial interface WithToken :
        IInteractionActor,
        IApplicationInteractionActor.WithToken
    {
        new IInteractionMessageActor.BackLink<WithToken> Response { get; }
        
        new IInteractionMessageActor.Indexable.BackLink<WithToken> FollowUps { get; }

        IInteractionMessageActor IInteractionTokenActor.Response => Response;
        IInteractionMessageActor.Indexable IInteractionTokenActor.FollowUps => FollowUps;
    }
    
    IApplicationInteractionActor.WithToken IApplicationInteractionActor.this[string token] => this[token];
}