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

    internal IActorProvider<TActor, TId> Provider { get; }
        = actorProvider;

    protected virtual RestLinkV2<TActor, TId, TEntity, TModel>[] Components { get; } = [];

    public TEntity CreateEntity(TModel model)
        => TEntity.Construct(Client, GetActor(model.Id), model);

    public TActor GetActor(TId id)
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
        public class Indexable(
            RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable indexable,
            Enumerable enumerable
        ) :
            RestLinkV2<TActor, TId, TEntity, TModel>(indexable.Client, indexable),
            ILinkType<TActor, TId, TEntity, TModel>.Enumerable.Indexable
        {
            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; } = indexable;
            internal Enumerable EnumerableLink { get; } = enumerable;

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, EnumerableLink];

            public TActor this[TId id] => Specifically(id);

            public TActor Specifically(TId id)
                => IndexableLink.Specifically(id);

            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => identity.Actor ?? this[identity.Id];
            
            public ITask<IReadOnlyCollection<TEntity>> AllAsync(
                RequestOptions? options = null,
                CancellationToken token = default
            ) => EnumerableLink.AllAsync(options, token);

            public Indexable(
                DiscordRestClient client,
                IActorProvider<TActor, TId> provider,
                EnumerableProviderDelegate enumerableProviderDelegate
            ) : this(new(client, provider), new(client, provider, enumerableProviderDelegate))
            {
            }
            
            public Indexable(
                DiscordRestClient client,
                IActorProvider<TActor, TId> provider,
                ApiModelProviderDelegate<IEnumerable<TModel>> enumerableProvider
            ) : this(new(client, provider), new(client, provider, enumerableProvider))
            {
            }
        }
    }

    public partial class Paged<TParams>
    {
        public class Indexable(
            RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable indexable,
            Paged<TParams> paged
        ) :
            RestLinkV2<TActor, TId, TEntity, TModel>(indexable.Client, indexable),
            ILinkType<TActor, TId, TEntity, TModel>.Paged<TParams>.Indexable
        {
            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; } = indexable;
            internal Paged<TParams> PagedLink { get; } = paged;

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, PagedLink];

            public TActor this[TId id] => Specifically(id);

            public TActor Specifically(TId id)
                => GetActor(id);
            
            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => identity.Actor ?? this[identity.Id];

            public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
                => PagedLink.PagedAsync(args, options);
        }
    }

    public partial class Paged<TPaged, TPagedModel, TParams>
    {
        public class Indexable(
            RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable indexable,
            Paged<TPaged, TPagedModel, TParams> paged
        ) :
            RestLinkV2<TActor, TId, TEntity, TModel>(indexable.Client, indexable),
            ILinkType<TActor, TId, TEntity, TModel>.Paged<TPaged, TParams>.Indexable
        {
            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; } = indexable;
            internal Paged<TPaged, TPagedModel, TParams> PagedLink { get; } = paged;

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, PagedLink];

            public TActor this[TId id] => Specifically(id);

            public TActor Specifically(TId id)
                => GetActor(id);
            
            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => identity.Actor ?? this[identity.Id];

            public IAsyncPaged<TPaged> PagedAsync(TParams? args = default, RequestOptions? options = null)
                => PagedLink.PagedAsync(args, options);
        }
    }

    public partial class Defined
    {
        public class Indexable(
            RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable indexable,
            Defined defined
        ) :
            RestLinkV2<TActor, TId, TEntity, TModel>(indexable.Client, indexable),
            ILinkType<TActor, TId, TEntity, TModel>.Defined.Indexable
        {
            internal RestLinkTypeV2<TActor, TId, TEntity, TModel>.Indexable IndexableLink { get; } = indexable;
            internal Defined DefinedLink { get; } = defined;

            protected override RestLinkV2<TActor, TId, TEntity, TModel>[] Components
                => [IndexableLink, DefinedLink];

            public TActor this[TId id] => Specifically(id);

            public TActor Specifically(TId id)
                => GetActor(id);
            
            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity]
                => identity.Actor ?? this[identity.Id];

            public IReadOnlyCollection<TId> Ids => DefinedLink.Ids;
        }
    }
}