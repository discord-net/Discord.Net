namespace Discord.Rest;

public partial class RestLinkTypeV2<TActor, TId, TEntity, TModel>
{
    public partial class Defined(
        DiscordRestClient client,
        IActorProvider<TActor, TId> actorProvider,
        IReadOnlyCollection<TId> ids
    ) :
        RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
        ILinkType<TActor, TId, TEntity, TModel>.Defined
    {
        public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;
    }
}