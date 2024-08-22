using Discord.Models;
using Discord.Paging;
using MorseCode.ITask;

namespace Discord.Rest;

[BackLinkable]
public partial class RestLinkType<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    RestActorProvider<TId, TActor> actorFactory
) :
    RestLink<TActor, TId, TEntity, TModel>(client, actorFactory),
    ILinkType<TActor, TId, TEntity, TModel>
    where TActor : class, IRestActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TModel : class, IEntityModel<TId>
{
    [BackLinkable]
    public partial class Indexable(
        DiscordRestClient client,
        RestActorProvider<TId, TActor> actorFactory
    ) :
        RestLink<TActor, TId, TEntity, TModel>(client, actorFactory),
        ILinkType<TActor, TId, TEntity, TModel>.Indexable
    {
        public TActor this[TId id] => Specifically(id);

        public TActor Specifically(TId id)
            => GetActor(id);
    }

    [BackLinkable]
    public partial class Enumerable(
        DiscordRestClient client,
        RestActorProvider<TId, TActor> actorFactory,
        ApiModelProviderDelegate<IEnumerable<TModel>> apiProvider
    ) :
        RestLink<TActor, TId, TEntity, TModel>(client, actorFactory),
        ILinkType<TActor, TId, TEntity, TModel>.Enumerable
    {
        public async ITask<IReadOnlyCollection<TEntity>> AllAsync(
            RequestOptions? options = null,
            CancellationToken token = default)
        {
            var models = await apiProvider(Client, options, token);

            return models
                .Select(CreateEntity)
                .ToList()
                .AsReadOnly();
        }

        [BackLinkable]
        public partial class Indexable(
            DiscordRestClient client,
            RestActorProvider<TId, TActor> actorFactory,
            ApiModelProviderDelegate<IEnumerable<TModel>> apiProvider
        ) :
            Enumerable(client, actorFactory, apiProvider),
            ILinkType<TActor, TId, TEntity, TModel>.Indexable
        {
            public TActor this[TId id] => Specifically(id);

            public TActor Specifically(TId id)
                => GetActor(id);
        }
    }

    [BackLinkable]
    public partial class Paged<TParams, TPageModel>(
        DiscordRestClient client,
        RestActorProvider<TId, TActor> actorFactory,
        IPathable path,
        Func<TPageModel, IEnumerable<TModel>> mapper
    ) :
        RestLink<TActor, TId, TEntity, TModel>(client, actorFactory),
        ILinkType<TActor, TId, TEntity, TModel>.Paged<TParams>
        where TParams : class, IPagingParams<TParams, TPageModel>
        where TPageModel : class
    {
        public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
        {
            return new RestPager<TId, TEntity, TModel, TPageModel, TParams>(
                Client,
                path,
                options ?? Client.DefaultRequestOptions,
                mapper,
                (model, _) => CreateEntity(model),
                args
            );
        }

        public partial class Indexable(
            DiscordRestClient client,
            RestActorProvider<TId, TActor> actorFactory,
            IPathable path,
            Func<TPageModel, IEnumerable<TModel>> mapper
        ) :
            Paged<TParams, TPageModel>(client, actorFactory, path, mapper),
            ILinkType<TActor, TId, TEntity, TModel>.Indexable
        {
            public TActor this[TId id] => Specifically(id);

            public TActor Specifically(TId id)
                => GetActor(id);
        }
    }

    [BackLinkable]
    public partial class Paged<TPaged, TPagedModel, TParams, TApiModel>(
        DiscordRestClient client,
        RestActorProvider<TId, TActor> actorFactory,
        IPathable path,
        Func<TApiModel, IEnumerable<TPagedModel>> mapper
    ) :
        RestLink<TActor, TId, TEntity, TModel>(client, actorFactory),
        ILinkType<TActor, TId, TEntity, TModel>.Paged<TPaged, TParams>
        where TParams : class, IPagingParams<TParams, TApiModel>
        where TApiModel : class
        where TPaged : class, IEntity<TId, TPagedModel>, IRestConstructable<TPaged, TActor, TPagedModel>
        where TPagedModel : class, IEntityModel<TId>
    {
        public IAsyncPaged<TPaged> PagedAsync(TParams? args = default, RequestOptions? options = null)
        {
            return new RestPager<TId, TPaged, TPagedModel, TApiModel, TParams>(
                Client,
                path,
                options ?? Client.DefaultRequestOptions,
                mapper,
                (model, _) => TPaged.Construct(Client, GetActor(model.Id), model),
                args
            );
        }

        [BackLinkable]
        public partial class Indexable(
            DiscordRestClient client,
            RestActorProvider<TId, TActor> actorFactory,
            IPathable path,
            Func<TApiModel, IEnumerable<TPagedModel>> mapper
        ) :
            Paged<TPaged, TPagedModel, TParams, TApiModel>(client, actorFactory, path, mapper),
            ILinkType<TActor, TId, TEntity, TModel>.Indexable
        {
            public TActor this[TId id] => Specifically(id);

            public TActor Specifically(TId id)
                => GetActor(id);
        }
    }

    [BackLinkable]
    public partial class Defined(
        DiscordRestClient client,
        RestActorProvider<TId, TActor> actorFactory,
        IReadOnlyCollection<TId> ids
    ) :
        RestLink<TActor, TId, TEntity, TModel>(client, actorFactory),
        ILinkType<TActor, TId, TEntity, TModel>.Defined
    {
        public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;

        [BackLinkable]
        public partial class Indexable(
            DiscordRestClient client,
            RestActorProvider<TId, TActor> actorFactory,
            IReadOnlyCollection<TId> ids
        ) :
            Defined(client, actorFactory, ids),
            ILinkType<TActor, TId, TEntity, TModel>.Indexable
        {
            public TActor this[TId id] => Specifically(id);

            internal TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity] =>
                identity.Actor ?? this[identity.Id];

            public TActor Specifically(TId id)
                => GetActor(id);
        }
    }
}