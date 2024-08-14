using Discord.Gateway.State;
using Discord.Models;
using Discord.Paging;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

internal sealed class CachePager<TId, TEntity, TModel, TParams> : IAsyncPaged<TEntity>
    where TId : IEquatable<TId>
    where TEntity :
    class,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    ICacheableEntity<TEntity, TId, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TParams : class, IPagingParams
{
    public int? PageSize => _pageSize;

    private readonly DiscordGatewayClient _client;
    private readonly CachePathable _path;

    private readonly Optional<TId> _upperBounds;
    private readonly Optional<TId> _lowerBounds;
    private readonly Direction? _direction;

    private readonly int _pageSize;
    private readonly int? _total;

    private IEntityBroker<TId, TEntity, TModel>? _broker;
    private IStoreInfo<TId, TModel>? _storeInfo;

    public CachePager(
        DiscordGatewayClient client,
        CachePathable path,
        TParams? pageParams)
    {
        _client = client;
        _path = path;

        if (pageParams is IDirectionalPagingParams<TId> directional)
        {
            if (directional is IBetweenPagingParams<TId> {IsBetween: true} between)
                _upperBounds = between.Before;

            _lowerBounds = directional.From;
            _direction = directional.Direction;
        }

        _pageSize = pageParams?.PageSize is not null
            ? Math.Min(pageParams.PageSize.Value, TParams.MaxPageSize)
            : TParams.MaxPageSize;

        _total = pageParams?.Total;
    }

    public async IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken token = default)
    {
        _storeInfo ??= await TEntity.GetStoreInfoAsync(_client, _path, token);
        _broker ??= TEntity.GetBroker(_client);

        var enumerator = _lowerBounds.IsSpecified && _direction.HasValue
            ? _broker.QueryAsync(
                _path,
                _storeInfo,
                _lowerBounds.Value,
                _upperBounds,
                _direction.Value,
                _total,
                token
            )
            : _broker.GetAllAsync(
                _path,
                _storeInfo,
                token
            );


        await foreach (var handle in enumerator.WithCancellation(token))
            yield return handle.ConsumeAsReference();
    }
}