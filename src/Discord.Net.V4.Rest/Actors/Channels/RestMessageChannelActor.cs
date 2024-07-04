using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using Discord.Rest.Messages;

namespace Discord.Rest;

public partial class RestLoadableMessageChannelActor(
    DiscordRestClient client,
    GuildIdentity? guild,
    IIdentifiableEntityOrModel<ulong, RestChannel, IChannelModel> channel) :
    RestMessageChannelActor(client, guild, channel),
    ILoadableMessageChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IMessageChannel>))]
    internal RestLoadable<ulong, IMessageChannel, IMessageChannel, Channel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(id),
            (_, model) =>
            {
                if (model is null)
                    return null;

                var entity = guild is not null
                    ? RestChannel.Construct(client, model, guild)
                    : RestChannel.Construct(client, model);

                if (entity is not IMessageChannel messageChannel)
                    throw new DiscordException(
                        $"Entity was of incorrect type: expected {typeof(IMessageChannel)}, got {entity.GetType()}");

                return messageChannel;
            }
        );
}

public partial class RestMessageChannelActor(
    DiscordRestClient client,
    GuildIdentity? guild,
    IIdentifiableEntityOrModel<ulong, RestChannel, IChannelModel> channel) :
    RestChannelActor(client, channel),
    IMessageChannelActor
{
    public RestIndexableActor<RestLoadableMessageActor, ulong, RestMessage> Messages { get; } =
        new(messageId => new(client, guild, channel, messageId));

    IIndexableActor<ILoadableMessageActor, ulong, IMessage> IMessageChannelActor.Messages => Messages;
}
