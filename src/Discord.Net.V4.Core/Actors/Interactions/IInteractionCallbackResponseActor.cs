using Discord.Rest;

namespace Discord;

[
    Modifiable<ModifyWebhookMessageProperties>(nameof(Routes.ModifyOriginalInteractionResponse)),
    Deletable(nameof(Routes.DeleteOriginalInteractionResponse))
]
public partial interface IInteractionCallbackResponseActor :
    IActor<ulong, IInteractionCallbackResponse>,
    ITokenPathProvider
{
    
}