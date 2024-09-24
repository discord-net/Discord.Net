namespace Discord.Rest;

public partial interface IRestChannelInvitableTrait :
    IRestTraitProvider<RestChannelActor>,
    IInvitableTrait<IChannelInviteActor, IChannelInvite>
{
    [SourceOfTruth]
    new RestChannelInviteActor.Enumerable.Indexable Invites => GetOrCreateTraitData(
        nameof(Invites),
        channel =>
        {
            var provider = RestActorProvider.GetOrCreate(
                channel.Client,
                Template.Of<ChannelInviteIdentity>(),
                channel.Identity
            );

            return new RestChannelInviteActor.Enumerable.Indexable(
                channel.Client,
                provider,
                Routes.GetChannelInvites(channel.Id)
                    .AsRequiredProvider()
                    .ToEntityEnumerableProvider(
                        Template.Of<ChannelInviteIdentity>(),
                        provider
                    )
            );
        });
}