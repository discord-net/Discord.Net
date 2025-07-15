using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Pipeline;

namespace Discord;

[
    Loadable<Routes.GetWebhookMessage>,
    Modifiable<Routes.UpdateWebhookMessage, ModifyWebhookMessageProperties>,
    Deletable<Routes.DeleteWebhookMessage>
]
public partial interface IInteractionMessageActor :
    IActor<ulong, IMessage>,
    IApplicationActor.CanonicalRelationship,
    ITokenPathProvider
{
    [BackLink<IInteractionActor.WithToken>]
    private static async Task<IInteractionCallbackResponse> CreateAsync(
        IInteractionActor.WithToken actor,
        CreateInteractionResponseProperties properties,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => await Routes.CreateInteractionResponse
        .Create(actor)
        .AsPipeline(properties.ToApiModel(), options)
        .Deserialize<IInteractionCallbackResponseModel>()
        .Required()
        .Transform(actor.CreateEntityAsync)
        .RunAsync(actor.Client, token);
}