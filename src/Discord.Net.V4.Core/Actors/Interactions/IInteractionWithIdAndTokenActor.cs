using Discord.Rest;

namespace Discord;

public partial interface IInteractionWithIdAndTokenActor :
    IInteractionWithTokenActor,
    IInteractionActor
{
    [SourceOfTruth]
    new ulong Id { get; }
    
    [SourceOfTruth]
    new IInteractionMessageActor.Indexable.WithOriginal Responses { get; }
    
    async Task<IInteractionCallbackResponse?> CallbackAsync(
        CreateInteractionResponseProperties properties,
        bool? withResponse = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteAsync(
            Routes.CreateInteractionResponse(
                Id,
                Token,
                properties.ToApiModel()
            ),
            options,
            token
        );

        return model is null 
            ? null 
            : Responses.Original.CreateEntity(model);
    }
}