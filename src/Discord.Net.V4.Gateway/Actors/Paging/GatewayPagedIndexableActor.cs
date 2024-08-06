using Discord.Gateway.State;
using Discord.Models;
using Discord.Paging;
using Discord.Rest;

namespace Discord.Gateway;

internal static partial class GatewayPagedIndexableActor
{
    public static GatewayPagedIndexableActor<
        TActor,
        TId,
        TGatewayEntity,
        TRestEntity,
        TCoreEntity,
        TModel,
        TApiModel,
        TParams
    > Create<
        [TransitiveFill] TActor,
        TId,
        TGatewayEntity,
        [Ignore]TRestEntity,
        [Interface, Not(nameof(TGatewayEntity)), Shrink] TCoreEntity,
        TModel,
        [Ignore]TParams,
        TApiModel
    >(
        Template<TActor> template,
        DiscordGatewayClient client,
        Func<TId, TActor> actorFactory,
        CachePathable path,
        Func<TApiModel, IEnumerable<TModel>> modelMapper,
        Func<TModel, TApiModel, TRestEntity> restEntityFactory,
        Func<TApiModel, RequestOptions, CancellationToken, ValueTask>? onRestPage = null
    )
        where TActor :
        class,
        IGatewayCachedActor<TId, TGatewayEntity, IIdentifiable<TId, TGatewayEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TGatewayEntity :
        GatewayEntity<TId>,
        TCoreEntity,
        ICacheableEntity<TGatewayEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TGatewayEntity, TModel>,
        IContextConstructable<TGatewayEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TRestEntity : RestEntity<TId>, TCoreEntity
        where TCoreEntity : class, IEntity<TId, TModel>
        where TModel : class, IEntityModel<TId>
        where TApiModel : class
        where TParams : class, IPagingParams<TParams, TApiModel>
    {
        return new GatewayPagedIndexableActor<TActor, TId, TGatewayEntity, TRestEntity, TCoreEntity, TModel, TApiModel,
            TParams>(
            client,
            actorFactory,
            path,
            modelMapper,
            restEntityFactory,
            onRestPage
        );
    }

    public static GatewayPartialPagedIndexableActor<
        TActor,
        TId,
        TGatewayEntity,
        TPartialRestEntity,
        TModel,
        TPartialModel,
        TApiModel,
        TParams
    > CreatePartial<
        [TransitiveFill] TActor,
        TId,
        TGatewayEntity,
        [Ignore]TPartialRestEntity,
        TModel,
        [Not(nameof(TModel)), Interface] TPartialModel,
        [Ignore]TParams,
        TApiModel
    >(
        Template<TActor> template,
        DiscordGatewayClient client,
        CachePathable path,
        Func<TId, TActor> actorFactory,
        Func<TApiModel, IEnumerable<TPartialModel>> modelMapper,
        Func<TPartialModel, TApiModel, TPartialRestEntity> restEntityFactory
    )
        where TActor :
        class,
        IGatewayCachedActor<TId, TGatewayEntity, IIdentifiable<TId, TGatewayEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TGatewayEntity :
        GatewayEntity<TId>,
        IEntity<TId, TModel>,
        ICacheableEntity<TGatewayEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TGatewayEntity, TModel>,
        IContextConstructable<TGatewayEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TPartialRestEntity : RestEntity<TId>, IEntityOf<TPartialModel>
        where TModel : class, TPartialModel
        where TPartialModel : class, IEntityModel<TId>
        where TApiModel : class
        where TParams : class, IPagingParams<TParams, TApiModel>
    {
        return new GatewayPartialPagedIndexableActor<
            TActor,
            TId,
            TGatewayEntity,
            TPartialRestEntity,
            TModel,
            TPartialModel,
            TApiModel,
            TParams
        >(
            client,
            path,
            actorFactory,
            modelMapper,
            restEntityFactory
        );
    }
}

public sealed class GatewayPartialPagedIndexableActor<
    TActor,
    TId,
    TGatewayEntity,
    TPartialRestEntity,
    TModel,
    TPartialModel,
    TApiModel,
    TParams
>(
    DiscordGatewayClient client,
    CachePathable path,
    Func<TId, TActor> actorFactory,
    Func<TApiModel, IEnumerable<TPartialModel>> modelMapper,
    Func<TPartialModel, TApiModel, TPartialRestEntity> restEntityFactory
) :
    IPagedIndexableActor<TActor, TId, TGatewayEntity, TPartialRestEntity, TParams>
    where TActor :
    class,
    IGatewayCachedActor<TId, TGatewayEntity, IIdentifiable<TId, TGatewayEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
    where TGatewayEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TGatewayEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TGatewayEntity, TModel>,
    IContextConstructable<TGatewayEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TPartialRestEntity : RestEntity<TId>, IEntityOf<TPartialModel>
    where TModel : class, TPartialModel
    where TPartialModel : class, IEntityModel<TId>
    where TApiModel : class
    where TParams : class, IPagingParams<TParams, TApiModel>
{
    private readonly GatewayIndexableActor<TActor, TId, TGatewayEntity> _indexableActor = new(actorFactory);

    private readonly GatewayPartialPagedActor<TId, TGatewayEntity, TPartialRestEntity, TModel, TPartialModel, TApiModel, TParams>
        _pagedActor = new(client, path, modelMapper, restEntityFactory);

    public TActor this[TId id] => _indexableActor[id];
    public TActor Specifically(TId id) => _indexableActor.Specifically(id);

    public IAsyncPaged<TPartialRestEntity> PageRestAsync(TParams? args = default, GatewayRequestOptions? options = null)
        => _pagedActor.PageRestAsync(args, options);

    public IAsyncPaged<TGatewayEntity> PageCacheAsync(TParams? args = default)
        => _pagedActor.PageCacheAsync(args);

    IAsyncPaged<TPartialRestEntity> IPagedActor<TId, TPartialRestEntity, TParams>.PagedAsync(TParams? args,
        RequestOptions? options)
        => PageRestAsync(args, GatewayRequestOptions.FromRestOptions(options));

    public static TActor operator >>(
        GatewayPartialPagedIndexableActor<TActor,
            TId,
            TGatewayEntity,
            TPartialRestEntity,
            TModel,
            TPartialModel,
            TApiModel,
            TParams
        > source,
        IIdentifiable<TId, TGatewayEntity, TActor, TModel> identity
    ) => identity.Actor ?? source[identity.Id];
}

public sealed class GatewayPagedIndexableActor<
    TActor,
    TId,
    TGatewayEntity,
    TRestEntity,
    TCoreEntity,
    TModel,
    TApiModel,
    TParams
>(
    DiscordGatewayClient client,
    Func<TId, TActor> actorFactory,
    CachePathable path,
    Func<TApiModel, IEnumerable<TModel>> modelMapper,
    Func<TModel, TApiModel, TRestEntity> restEntityFactory,
    Func<TApiModel, RequestOptions, CancellationToken, ValueTask>? onRestPage = null
) :
    IPagedIndexableActor<TActor, TId, TCoreEntity, TParams>
    where TActor :
    class,
    IGatewayCachedActor<TId, TGatewayEntity, IIdentifiable<TId, TGatewayEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
    where TGatewayEntity :
    GatewayEntity<TId>,
    TCoreEntity,
    ICacheableEntity<TGatewayEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TGatewayEntity, TModel>,
    IContextConstructable<TGatewayEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TRestEntity : RestEntity<TId>, TCoreEntity
    where TCoreEntity : class, IEntity<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TApiModel : class
    where TParams : class, IPagingParams<TParams, TApiModel>
{
    private readonly GatewayIndexableActor<TActor, TId, TGatewayEntity> _indexableActor = new(actorFactory);

    private readonly GatewayPagedActor<TId, TGatewayEntity, TRestEntity, TCoreEntity, TModel, TApiModel, TParams>
        _pagedActor = new(client, path, modelMapper, restEntityFactory, onRestPage);

    public TActor this[TId id] => _indexableActor[id];

    public TActor Specifically(TId id) => _indexableActor.Specifically(id);

    public IAsyncPaged<TCoreEntity> PagedAsync(TParams? args = default, GatewayRequestOptions? options = null)
        => _pagedActor.PagedAsync(args, options);

    public IAsyncPaged<TRestEntity> PageRestAsync(TParams? args = default, GatewayRequestOptions? options = null)
        => _pagedActor.PageRestAsync(args, options);

    public IAsyncPaged<TGatewayEntity> PageCacheAsync(TParams? args = default)
        => _pagedActor.PageCacheAsync(args);

    IAsyncPaged<TCoreEntity> IPagedActor<TId, TCoreEntity, TParams>.PagedAsync(TParams? args, RequestOptions? options)
        => _pagedActor.PagedAsync(args, GatewayRequestOptions.FromRestOptions(options));

    public static TActor operator >>(
        GatewayPagedIndexableActor<
            TActor,
            TId,
            TGatewayEntity,
            TRestEntity,
            TCoreEntity,
            TModel,
            TApiModel,
            TParams
        > source,
        IIdentifiable<TId, TGatewayEntity, TActor, TModel> identity
    ) => identity.Actor ?? source[identity.Id];
}
