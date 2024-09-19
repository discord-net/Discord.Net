using MorseCode.ITask;

namespace Discord.Rest;

public partial class RestLinkTypeV2<TActor, TId, TEntity, TModel>
{
    public partial class Enumerable :
        RestLinkV2<TActor, TId, TEntity, TModel>,
        ILinkType<TActor, TId, TEntity, TModel>.Enumerable
    {
        public delegate ITask<IReadOnlyCollection<TEntity>> EnumerableProviderDelegate(
            DiscordRestClient client,
            RequestOptions? options = null,
            CancellationToken token = default
        );

        internal EnumerableProviderDelegate EnumerableProvider { get; }

        internal Enumerable(
            DiscordRestClient client,
            IActorProvider<TActor, TId> actorProvider,
            ApiModelProviderDelegate<IEnumerable<TModel>> modelProvider
        ) : base(client, actorProvider)
        {
            EnumerableProvider = async (client, options, token) =>
            {
                var models = await modelProvider(client, options, token);

                return models
                    .Select(CreateEntity)
                    .ToList()
                    .AsReadOnly();
            };
        }

        public Enumerable(
            DiscordRestClient client,
            IActorProvider<TActor, TId> actorProvider,
            EnumerableProviderDelegate provider
        ) : base(client, actorProvider)
        {
            EnumerableProvider = provider;
        }

        public ITask<IReadOnlyCollection<TEntity>> AllAsync(
            RequestOptions? options = null,
            CancellationToken token = default)
            => EnumerableProvider(Client, options, token);
    }
}