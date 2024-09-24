namespace Discord.Rest;

public partial class RestLinkTypeV2<TActor, TId, TEntity, TModel>
{
    public partial class Indexable(
        DiscordRestClient client,
        IActorProvider<TActor, TId> actorProvider
    ) :
        RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
        ILinkType<TActor, TId, TEntity, TModel>.Indexable
    {
        public virtual TActor this[TId id] => Specifically(id);
        public virtual TActor Specifically(TId id) => GetActor(id);

        internal virtual TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
            => Specifically(identity);
        
        internal virtual TActor Specifically(IIdentifiable<TId, TEntity, TActor, TModel> identity)
            => identity.Actor ?? this[identity.Id];
    }
}