using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using Discord.Rest.Messages;

namespace Discord.Rest;

public partial class RestMessageChannelActor(
    DiscordRestClient client,
    MessageChannelIdentity channel,
    GuildIdentity? guild = null
):
    RestChannelActor(client, channel.Cast<RestChannel, RestChannelActor, IChannelModel>()),
    IMessageChannelActor
{
    internal MessageChannelIdentity MessageChannelIdentity { get; } = channel;

    [SourceOfTruth]
    public RestIndexableActor<RestMessageActor, ulong, RestMessage> Messages { get; } =
        new(messageId => new(client, channel, MessageIdentity.Of(messageId), guild));

    IMessageChannel IMessageChannelActor.CreateEntity(IChannelModel model)
        => (IMessageChannel)CreateEntity(model);

    IMessageChannel IEntityProvider<IMessageChannel, IChannelModel>.CreateEntity(IChannelModel model)
        => (IMessageChannel)CreateEntity(model);
}
