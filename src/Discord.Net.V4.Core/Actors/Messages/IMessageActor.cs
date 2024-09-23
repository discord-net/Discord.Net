using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannelMessage)),
    Deletable(nameof(Routes.DeleteMessage)),
    Creatable<CreateMessageProperties>(
        nameof(Routes.CreateMessage),
        nameof(IMessageChannelTrait)
    ),
    Modifiable<ModifyMessageProperties>(nameof(Routes.ModifyMessage))
]
public partial interface IMessageActor :
    IChannelRelationship<IMessageChannelTrait, IMessageChannel>,
    IActor<ulong, IMessage>
{
    IPollActor Poll { get; }
    IReactionActor.Indexable.BackLink<IMessageActor> Reactions { get; }

    [BackLink<IGuildChannelActor>]
    private static Task BulkDeleteAsync(
        IGuildChannelActor channel,
        IEnumerable<EntityOrId<ulong, IMessageActor>> messages,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var ids = messages.Select(x => x.Id).ToArray();

        if (ids.Length is < DiscordConfig.MinBulkDeleteMessages or > DiscordConfig.MaxMessagesPerBatch)
            throw new ArgumentOutOfRangeException(
                nameof(messages),
                ids.Length,
                $"{nameof(ids)} must contain at least {DiscordConfig.MinBulkDeleteMessages} ids and at most " +
                $"{DiscordConfig.MaxBulkDeleteMessages} ids."
            );

        return channel.Client.RestApiClient.ExecuteAsync(
            Routes.BulkDeleteMessages(
                new BulkDeleteMessagesParams()
                {
                    Messages = ids
                },
                channel.Id
            ),
            options ?? channel.Client.DefaultRequestOptions,
            token
        );
    }
}