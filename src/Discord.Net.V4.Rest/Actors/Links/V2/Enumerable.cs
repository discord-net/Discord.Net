using MorseCode.ITask;

namespace Discord.Rest;

public delegate ITask<IReadOnlyCollection<TEntity>> EnumerableProviderDelegate<out TEntity>(
    DiscordRestClient client,
    RequestOptions? options = null,
    CancellationToken token = default
);

public partial class RestLinkTypeV2<TActor, TId, TEntity, TModel>
{
    public partial class Enumerable(
        DiscordRestClient client,
        IActorProvider<TActor, TId> actorProvider,
        EnumerableProviderDelegate<TEntity> provider
    ) :
        RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
        ILinkType<TActor, TId, TEntity, TModel>.Enumerable
    {
        internal virtual EnumerableProviderDelegate<TEntity> EnumerableProvider { get; } = provider;

        public virtual ITask<IReadOnlyCollection<TEntity>> AllAsync(
            RequestOptions? options = null,
            CancellationToken token = default)
            => EnumerableProvider(Client, options, token);
    }
}