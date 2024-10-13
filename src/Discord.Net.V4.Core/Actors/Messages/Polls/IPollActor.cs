using Discord.Rest;

namespace Discord;

public partial interface IPollActor :
    IActor<ulong, IPoll>,
    IMessageActor.CanonicalRelationship
{
    IPollAnswerActor.Indexable Answers { get; }
    
    async Task<IMessage> EndAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.EndPoll(
                Channel.Id,
                Message.Id
            ),
            options,
            token
        );

        return Message.CreateEntity(model);
    }
}