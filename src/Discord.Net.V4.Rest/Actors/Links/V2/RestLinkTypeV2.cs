using System.Diagnostics.CodeAnalysis;
using Discord.Models;
using MorseCode.ITask;

namespace Discord.Rest;

public abstract class RestLinkV2<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    IActorProvider<TActor, TId> actorProvider
) :
    IRestClientProvider,
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IRestActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TModel : class, IEntityModel<TId>
{
    public DiscordRestClient Client { get; } = client;

    internal virtual IActorProvider<TActor, TId> Provider { get; }
        = actorProvider;

    protected virtual RestLinkV2<TActor, TId, TEntity, TModel>[] Components { get; } = [];

    public virtual TEntity CreateEntity(TModel model)
        => TEntity.Construct(Client, GetActor(model.Id), model);

    public virtual TActor GetActor(TId id)
        => Provider.GetActor(id);

    internal bool TryGetComponent<T>([MaybeNullWhen(false)] out T component)
        where T : RestLinkV2<TActor, TId, TEntity, TModel>
    {
        if (this is T self)
        {
            component = self;
            return true;
        }

        foreach (var componentInstance in Components)
        {
            if (componentInstance is not T inst) continue;

            component = inst;
            return true;
        }

        component = default;
        return false;
    }
}

public partial class RestLinkTypeV2<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    IActorProvider<TActor, TId> actorProvider
) :
    RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
    ILinkType<TActor, TId, TEntity, TModel>
    where TActor : class, IRestActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TModel : class, IEntityModel<TId>
{
    public partial class Indexable;

    public partial class Enumerable
    {
        public class Indexable :
            RestLinkV2<TActor, TId, TEntity, TModel>,
            ILinkType<TActor, TId, TEntity, TModel>.Enumerable.Indexable
        {
            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; }
            internal Enumerable EnumerableLink { get; }

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, EnumerableLink];

            public virtual TActor this[TId id] => Specifically(id);

            public virtual TActor Specifically(TId id)
                => IndexableLink.Specifically(id);

            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => identity.Actor ?? this[identity.Id];

            public virtual ITask<IReadOnlyCollection<TEntity>> AllAsync(
                RequestOptions? options = null,
                CancellationToken token = default
            ) => EnumerableLink.AllAsync(options, token);

            public Indexable(
                DiscordRestClient client,
                IActorProvider<TActor, TId> provider,
                EnumerableProviderDelegate<TEntity> enumerableProviderDelegate
            ) : base(client, provider)
            {
                IndexableLink = new(client, provider);
                EnumerableLink = new(client, provider, enumerableProviderDelegate);
            }
        }
    }

    public partial class Paged<TParams>
    {
        public class Indexable :
            RestLinkV2<TActor, TId, TEntity, TModel>,
            ILinkType<TActor, TId, TEntity, TModel>.Paged<TParams>.Indexable
        {
            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; }
            internal Paged<TParams> PagedLink { get; }

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, PagedLink];

            public Indexable(
                DiscordRestClient client,
                IActorProvider<TActor, TId> actorProvider,
                IPagingProvider<TParams, TEntity, TModel> pagingProvider
            ) : base(client, actorProvider)
            {
                IndexableLink = new(client, actorProvider);
                PagedLink = new(client, actorProvider, pagingProvider);
            }

            public virtual TActor this[TId id] => Specifically(id);

            public virtual TActor Specifically(TId id)
                => GetActor(id);

            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => Specifically(identity);

            internal TActor Specifically(IIdentifiable<TId, TEntity, TActor, TModel> identity)
                => identity.Actor ?? this[identity.Id];

            public virtual IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
                => PagedLink.PagedAsync(args, options);
        }
    }

    public partial class Paged<TPaged, TPagedModel, TParams>
    {
        public class Indexable :
            RestLinkV2<TActor, TId, TEntity, TModel>,
            ILinkType<TActor, TId, TEntity, TModel>.Paged<TPaged, TParams>.Indexable
        {
            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; }
            internal Paged<TPaged, TPagedModel, TParams> PagedLink { get; }

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, PagedLink];

            public Indexable(
                DiscordRestClient client,
                IActorProvider<TActor, TId> actorProvider,
                IPagingProvider<TParams, TPaged, TPagedModel> pagingProvider
            ) : base(client, actorProvider)
            {
                IndexableLink = new(client, actorProvider);
                PagedLink = new(client, actorProvider, pagingProvider);
            }

            public virtual TActor this[TId id] => Specifically(id);

            public virtual TActor Specifically(TId id)
                => GetActor(id);

            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => identity.Actor ?? this[identity.Id];

            public virtual IAsyncPaged<TPaged> PagedAsync(TParams? args = default, RequestOptions? options = null)
                => PagedLink.PagedAsync(args, options);
        }
    }

    public partial class Defined
    {
        public class Indexable :
            RestLinkV2<TActor, TId, TEntity, TModel>,
            ILinkType<TActor, TId, TEntity, TModel>.Defined.Indexable
        {
            public IReadOnlyCollection<TId> Ids => DefinedLink.Ids;

            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; }
            internal Defined DefinedLink { get; }

            public Indexable(
                DiscordRestClient client,
                IActorProvider<TActor, TId> provider,
                IReadOnlyCollection<TId> ids
            ) : base(client, provider)
            {
                IndexableLink = new(client, provider);
                DefinedLink = new(client, provider, ids);
            }

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, DefinedLink];

            public virtual TActor this[TId id] => Specifically(id);

            public virtual TActor Specifically(TId id)
                => GetActor(id);

            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => identity.Actor ?? this[identity.Id];
        }
    }
}