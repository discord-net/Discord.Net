using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest;

namespace Discord.Rest;

using RestMessages = RestPagedIndexableActor<
    RestMessageActor,
    ulong,
    RestMessage,
    IMessageModel,
    IEnumerable<IMessageModel>,
    PageChannelMessagesParams
>;

public sealed partial class RestMessageChannelTrait<TChannelActor, TIdentity>(
    DiscordRestClient client,
    TChannelActor channel,
    TIdentity identity
) :
    RestMessageChannelTrait(client, channel, identity)
    where TChannelActor : RestChannelActor
    where TIdentity : class, MessageChannelIdentity
{
    [ProxyInterface] internal override TChannelActor Channel { get; } = channel;

    internal override TIdentity Identity { get; } = identity;
}

public partial class RestMessageChannelTrait :
    RestTrait<ulong, RestChannel, MessageChannelIdentity>,
    IMessageChannelTrait
{
    [SourceOfTruth] public RestMessages Messages { get; }

    [ProxyInterface] internal virtual RestChannelActor Channel { get; }

    internal override MessageChannelIdentity Identity { get; }

    public RestMessageChannelTrait(
        DiscordRestClient client,
        RestChannelActor channel,
        MessageChannelIdentity identity
    ) : base(client, identity)
    {
        Identity = identity;
        Channel = channel;

        var guild = (channel as RestGuildChannelActor)?.Guild.Identity;

        Messages = new RestMessages(
            client,
            id => new RestMessageActor(client, Identity, MessageIdentity.Of(id), guild),
            channel,
            x => x,
            (model, _) => RestMessage.Construct(
                client,
                new RestMessage.Context(
                    guild,
                    identity
                ),
                model
            )
        );
    }

    [SourceOfTruth]
    internal IMessageChannel CreateEntity(IChannelModel model)
        => (IMessageChannel)Channel.CreateEntity(model);
}
