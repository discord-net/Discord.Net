using Discord.Models;

namespace Discord.Rest;

public sealed class RestInvitableTrait<TBackLink, TInviteActor, TInvite> :
    RestInvitableTrait
    where TBackLink : class, IPathable
    where TInvite : RestInvite, IRestConstructable<TInvite, TInviteActor, IInviteModel>
    where TInviteActor : RestInviteActor, IRestActor<string, TInvite, IInviteModel>
{
    public override ILinkType<TInviteActor, string, TInvite, IInviteModel>.Enumerable.Indexable.BackLink<TBackLink>
        Invites { get; }

    public RestInvitableTrait(
        TBackLink backLink,
        DiscordRestClient client,
        IApiOutRoute<IEnumerable<IInviteModel>> route,
        IActorProvider<DiscordRestClient, TInviteActor, string> provider)
        : base(client, route)
    {
        Invites =
            new RestLinkType<TInviteActor, string, TInvite, IInviteModel>.Enumerable.Indexable.BackLink<TBackLink>(
                backLink,
                client,
                provider,
                FetchAsync
            );
    }
}

public partial class RestInvitableTrait : IInvitableTrait
{
    [SourceOfTruth] public virtual InviteLink.Enumerable.Indexable Invites { get; }

    private readonly IApiOutRoute<IEnumerable<IInviteModel>> _route;

    public RestInvitableTrait(
        DiscordRestClient client,
        IApiOutRoute<IEnumerable<IInviteModel>> route)
    {
        _route = route;

        Invites = new RestInviteLink.Enumerable.Indexable(
            client,
            client.Invites,
            FetchAsync
        );
    }

    protected async Task<IEnumerable<IInviteModel>> FetchAsync(
        DiscordRestClient client,
        RequestOptions? options,
        CancellationToken token)
    {
        var result = await client.RestApiClient.ExecuteAsync(_route, options, token);

        return result ?? [];
    }
}