using Discord.Models;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetFollowupMessage)),
    Modifiable<ModifyWebhookMessageProperties>(nameof(Routes.ModifyFollowupMessage)),
    Deletable(nameof(Routes.DeleteFollowupMessage))
]
public partial interface IInteractionMessageActor :
    IActor<ulong, IMessage>,
    IApplicationActor.CanonicalRelationship,
    ITokenPathProvider
{

    [LinkExtension]
    private interface WithOriginalExtension
    {
        IInteractionCallbackResponseActor Original { get; }
    }
}