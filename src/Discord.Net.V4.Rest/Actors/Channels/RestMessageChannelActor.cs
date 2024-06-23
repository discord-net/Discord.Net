using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Messages;

namespace Discord.Rest;

public partial class RestLoadableMessageChannelActor(DiscordRestClient client, ulong? guildId, ulong id) :
    RestMessageChannelActor(client, guildId, id),
    ILoadableMessageChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IMessageChannel>))]
    internal RestLoadable<ulong, IMessageChannel, IMessageChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            (_, model) =>
            {
                if (model is null)
                    return null;

                var entity = guildId.HasValue
                    ? RestChannel.Construct(client, model, guildId.Value)
                    : RestChannel.Construct(client, model);

                if (entity is not IMessageChannel messageChannel)
                    throw new DiscordException(
                        $"Entity was of incorrect type: expected {typeof(IMessageChannel)}, got {entity.GetType()}");

                return messageChannel;
            }
        );
}

public partial class RestMessageChannelActor(DiscordRestClient client, ulong? guildId, ulong id) :
    RestChannelActor(client, id),
    IMessageChannelActor
{
    public RestIndexableActor<RestLoadableMessageActor, ulong, RestMessage> Messages { get; } =
        new(messageId => new(client, guildId, id, messageId));

    IIndexableActor<ILoadableMessageActor, ulong, IMessage> IMessageChannelActor.Messages => Messages;
}
