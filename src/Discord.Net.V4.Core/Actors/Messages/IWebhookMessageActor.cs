using Discord.Models;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetWebhookMessage)),
    Modifiable<ModifyWebhookMessageProperties>(nameof(Routes.ModifyWebhookMessage)),
    Deletable(nameof(Routes.DeleteWebhookMessage))
]
public partial interface IWebhookMessageActor :
    IMessageActor,
    IWebhookRelationship,
    IActor<ulong, IWebhookMessage>,
    ITokenPathProvider
{
    [SourceOfTruth]
    new ulong Id { get; }
}
