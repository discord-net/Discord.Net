global using MessageChannelIdentity =
    Discord.IIdentifiable<ulong, Discord.IMessageChannel, Discord.Rest.IRestMessageChannelTrait,
        Discord.Models.IChannelModel>;
using Discord.Models;

namespace Discord.Rest;

[Containerized, Trait]
public partial interface IRestMessageChannelTrait :
    IMessageChannelTrait,
    IRestTraitProvider<RestChannelActor>,
    IRestTrait<ulong, IRestMessageChannel>
{
    [SourceOfTruth]
    new RestMessageActor.Paged<PageChannelMessagesParams>.Indexable Messages
        => GetOrCreateTraitData(nameof(Messages), channel =>
            RestMessageActor.Paged<PageChannelMessagesParams>.Indexable.Create(
                RestMessageActor.DefaultPagingProvider,
                channel.Client,
                RestMessageActor.GetProvider(channel.Client, (MessageChannelIdentity) channel.Identity)
            )
        );
}