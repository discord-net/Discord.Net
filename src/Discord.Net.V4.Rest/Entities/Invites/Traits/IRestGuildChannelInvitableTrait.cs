namespace Discord.Rest;

public partial interface IRestGuildChannelInvitableTrait :
    IRestTraitProvider<RestGuildChannelActor>,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>
{
    [SourceOfTruth]
    new RestGuildChannelInviteActor.Enumerable.Indexable.BackLink<RestGuildChannelActor> Invites
        => GetOrCreateTraitData(nameof(Invites), channel =>
            new RestGuildChannelInviteActor.Enumerable.Indexable.BackLink<RestGuildChannelActor>(
                channel,
                channel.Client,
                RestActorProvider.GetOrCreate(
                    channel.Client,
                    Template.Of<GuildChannelInviteIdentity>(),
                    channel.Guild.Identity,
                    channel.Identity
                ),
                IGuildChannelInvite.FetchManyRoute(channel)
                    .AsRequiredProvider()
            )
        );
}