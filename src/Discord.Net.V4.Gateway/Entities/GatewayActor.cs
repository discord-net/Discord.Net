using Discord.Gateway.Cache;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

public abstract class GatewayCachedActor<TId, TEntity, TIdentity, TModel>(
    DiscordGatewayClient client,
    TIdentity identity
) :
    GatewayActor<TId, TEntity, TIdentity>(client, identity),
    IGatewayCachedActor<TId, TEntity, TIdentity, TModel>,
    IAsyncDisposable
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>
    where TIdentity : IIdentifiable<TId>
    where TModel : class, IEntityModel<TId>
{
    protected readonly SemaphoreSlim StateSemaphore = new(1, 1);

    internal abstract ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync(CancellationToken token = default);

    private LinkedList<Func<ValueTask>>? _disposeTasks;

    protected void RegisterDisposeTask(Func<ValueTask> task)
        => (_disposeTasks ??= new()).AddLast(task);

    public virtual async ValueTask DisposeAsync()
    {
        // TODO: handle

        if (_disposeTasks is not null)
        {
            foreach (var task in _disposeTasks)
            {
                await task();
            }
        }
    }

    ValueTask<IEntityModelStore<TId, TModel>> IStoreProvider<TId, TModel>.GetStoreAsync(
        CancellationToken token
    ) => GetStoreAsync(token);
}

public interface IGatewayCachedActor<TId, out TEntity, out TIdentity, TModel> :
    IGatewayActor<TId, TEntity, TIdentity>,
    IStoreProvider<TId, TModel>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>
    where TIdentity : IIdentifiable<TId>
    where TModel : class, IEntityModel<TId>;

public abstract class GatewayActor<TId, TEntity, TIdentity>(
    DiscordGatewayClient client,
    TIdentity identity
) :
    IGatewayActor<TId, TEntity, TIdentity>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>
    where TIdentity : IIdentifiable<TId>
{
    public DiscordGatewayClient Client { get; } = client;

    public TId Id { get; } = identity.Id;

    public virtual TIdentity Identity { get; } = identity;

    public static implicit operator TIdentity(GatewayActor<TId, TEntity, TIdentity> actor) => actor.Identity;
}

public interface IGatewayActor<out TId, out TEntity, out TIdentity> : IGatewayActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    TIdentity Identity { get; }
}

public interface IGatewayActor<out TId, out TEntity> : IActor<TId, TEntity>, IGatewayClientProvider
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;
