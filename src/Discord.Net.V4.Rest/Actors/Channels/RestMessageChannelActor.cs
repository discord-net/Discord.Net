using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using Discord.Rest.Messages;

namespace Discord.Rest;

public partial class RestLoadableMessageChannelActor(
    DiscordRestClient client,
    MessageChannelIdentity channel
) :
    RestMessageChannelActor(client, channel),
    ILoadableMessageChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IMessageChannel>))]
    internal RestLoadable<ulong, IMessageChannel, IMessageChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            (client, _, model) =>
            {
                if (model is null)
                    return null;

                var entity = RestChannel.Construct(client, model);

                if (entity is not IMessageChannel messageChannel)
                    throw new DiscordException(
                        $"Entity was of incorrect type: expected {typeof(IMessageChannel)}, got {entity.GetType()}");

                return messageChannel;
            }
        );
}

public partial class RestMessageChannelActor(
    DiscordRestClient client,
    MessageChannelIdentity channel
) :
    RestChannelActor(client, ChannelIdentity.Of(channel.Id)),
    IMessageChannelActor
{
    public RestIndexableActor<RestLoadableMessageActor, ulong, RestMessage> Messages { get; } =
        new(messageId => new(client, channel, MessageIdentity.Of(messageId)));

    IIndexableActor<ILoadableMessageActor, ulong, IMessage> IMessageChannelActor.Messages => Messages;
}
