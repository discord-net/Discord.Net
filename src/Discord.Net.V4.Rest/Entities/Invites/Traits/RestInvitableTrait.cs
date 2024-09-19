using Discord.Models;

namespace Discord.Rest;

public sealed partial class RestInvitableTrait<TInviteActor, TInvite> :
    RestInvitableTrait,
    IInvitableTrait<TInviteActor, TInvite>
    where TInvite : RestInvite, IRestConstructable<TInvite, TInviteActor, IInviteModel>
    where TInviteActor : RestInviteActor, IRestActor<string, TInvite, IInviteModel>
{
    [SourceOfTruth]
    public override ILinkType<TInviteActor, string, TInvite, IInviteModel>.Enumerable.Indexable Invites { get; }

    public RestInvitableTrait(
        DiscordRestClient client,
        IActorProvider<TInviteActor, string> provider,
        IApiOutRoute<IEnumerable<IInviteModel>> route
    ) : base(client, provider, route)
    {
        Invites = new RestLinkTypeV2<TInviteActor, string, TInvite, IInviteModel>
            .Enumerable
            .Indexable(
                client,
                provider,
                route.AsRequiredProvider()
            );
    }

    // public override ILinkType<TInviteActor, string, TInvite, IInviteModel>.Enumerable.Indexable.BackLink<TBackLink>
    //     Invites { get; }
    //
    // public RestInvitableTrait(
    //     TBackLink backLink,
    //     DiscordRestClient client,
    //     IApiOutRoute<IEnumerable<IInviteModel>> route,
    //     IActorProvider<DiscordRestClient, TInviteActor, string> provider)
    //     : base(client, route)
    // {
    //     Invites =
    //         new RestLinkType<TInviteActor, string, TInvite, IInviteModel>.Enumerable.Indexable.BackLink<TBackLink>(
    //             backLink,
    //             client,
    //             provider,
    //             FetchAsync
    //         );
    // }
}

public partial class RestInvitableTrait : IInvitableTrait
{
    [SourceOfTruth] public virtual InviteLinkType.Enumerable.Indexable Invites { get; }

    public RestInvitableTrait(
        DiscordRestClient client,
        IActorProvider<RestInviteActor, string> provider,
        IApiOutRoute<IEnumerable<IInviteModel>> route
    )
    {
        Invites = new RestLinkTypeV2<RestInviteActor, string, RestInvite, IInviteModel>
            .Enumerable
            .Indexable(
                client,
                provider,
                route.AsRequiredProvider()
            );
    }
}