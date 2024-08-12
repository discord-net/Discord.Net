using Discord.Gateway;
using Discord.Gateway.State;
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
    where TEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TIdentity : IIdentifiable<TId>
    where TModel : class, IEntityModel<TId>
{
    protected readonly SemaphoreSlim StateSemaphore = new(1, 1);

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
}

public interface IGatewayCachedActor<out TId, out TEntity, out TIdentity, TModel> :
    IGatewayActor<TId, TEntity, TIdentity>,
    IGatewayCachedActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TIdentity : IIdentifiable<TId>
    where TModel : class, IEntityModel<TId>;

public interface IGatewayCachedActor<out TId, out TEntity, TModel> :
    IGatewayActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
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

    internal abstract TIdentity Identity { get; }

    TIdentity IGatewayActor<TId, TEntity, TIdentity>.Identity => Identity;

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
