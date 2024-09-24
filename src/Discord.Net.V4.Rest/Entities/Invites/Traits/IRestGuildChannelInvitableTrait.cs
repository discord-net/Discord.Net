namespace Discord.Rest;

public partial interface IRestGuildChannelInvitableTrait :
    IRestTraitProvider<RestGuildChannelActor>,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>
{
    [SourceOfTruth]
    new RestGuildChannelInviteActor.Enumerable.Indexable.BackLink<RestGuildChannelActor> Invites
        => GetOrCreateTraitData(nameof(Invites), channel =>
            {
                var provider = RestActorProvider.GetOrCreate(
                    channel.Client,
                    Template.Of<GuildChannelInviteIdentity>(),
                    channel.Guild.Identity,
                    channel.Identity
                );

                return new RestGuildChannelInviteActor.Enumerable.Indexable.BackLink<RestGuildChannelActor>(
                    channel,
                    channel.Client,
                    provider,
                    IGuildChannelInvite
                        .FetchManyRoute(channel)
                        .AsRequiredProvider()
                        .ToEntityEnumerableProvider(
                            Template.Of<GuildChannelInviteIdentity>(),
                            provider
                        )
                );
            }
        );
}