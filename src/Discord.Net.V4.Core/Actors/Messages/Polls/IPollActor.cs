using Discord.Models;
using Discord.Rest;
using Discord.Rest.Pipeline;

namespace Discord;

public partial interface IPollActor :
    IActor<ulong, IPoll>,
    IMessageActor.CanonicalRelationship
{
    IPollAnswerActor.Indexable Answers { get; }

    ValueTask<IMessage> EndAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => Routes
            .PollExpire
            .Create(this)
            .AsPipeline(options)
            .Deserialize<IMessageModel>()
            .Transform(Message);

    // async Task<IMessage> EndAsync(RequestOptions? options = null, CancellationToken token = default)
    // {
    //     var model = await Client.RestApiClient.ExecuteRequiredAsync(
    //         Routes.EndPoll(
    //             Message.Channel.Id,
    //             Message.Id
    //         ),
    //         options,
    //         token
    //     );
    //
    //     return await Message.CreateEntityAsync(model, token);
    // }
}