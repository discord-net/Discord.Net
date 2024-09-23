using Discord.Models;

namespace Discord.Rest;

[Containerized, Trait]
public partial interface IRestMessageChannelTrait :
    IMessageChannelTrait,
    IRestTraitProvider<RestChannelActor>
{
    [SourceOfTruth]
    new RestMessageActor.Paged<PageChannelMessagesParams>.Indexable Messages
        => GetOrCreateTraitData(nameof(Messages), channel =>
        {
            var provider = RestActorProvider.GetOrCreate(
                channel.Client,
                Template.Of<MessageIdentity>(),
                (MessageChannelIdentity) channel.Identity
            );

            return new RestMessageActor.Paged<PageChannelMessagesParams>.Indexable(
                new(
                    channel.Client,
                    provider
                ),
                new(
                    channel.Client,
                    provider,
                    new RestPagingProvider<IMessageModel, PageChannelMessagesParams, RestMessage>(
                        channel.Client,
                        (model, _) => Messages[model.Id].CreateEntity(model),
                        this
                    )
                )
            );
        });
}