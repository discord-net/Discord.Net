namespace Discord.Rest;

public partial interface IRestChannelInvitableTrait :
    IRestTraitProvider<RestChannelActor>,
    IInvitableTrait<IChannelInviteActor, IChannelInvite>
{
    [SourceOfTruth]
    new RestChannelInviteActor.Enumerable.Indexable Invites => GetOrCreateTraitData(
        nameof(Invites),
        channel => new RestChannelInviteActor.Enumerable.Indexable(
            channel.Client,
            RestActorProvider.GetOrCreate(
                channel.Client,
                Template.Of<ChannelInviteIdentity>(),
                channel.Identity
            ),
            Routes.GetChannelInvites(channel.Id)
                .AsRequiredProvider()
        )
    );
}