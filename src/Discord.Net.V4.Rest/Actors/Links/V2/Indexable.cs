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
        public TActor this[TId id] => Specifically(id);
        public TActor Specifically(TId id) => GetActor(id);

        internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
            => identity.Actor ?? this[identity.Id];
    }
}