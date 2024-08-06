using Discord.Models;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway.State;

/// <summary>
///     Represents a weak reference to an entity. This instance does not ensure that the entity it represents is
///     still alive in memory, see <see cref="IEntityHandle{TId,TEntity}"/> for that.
/// </summary>
/// <typeparam name="TId">The ID type of the entity that this reference represents.</typeparam>
internal interface IEntityReference<out TId>
{
    TId Id { get; }
}

/// <summary>
///     Represents a weak reference to an entity. This instance does not ensure that the entity it represents is
///     still alive in memory, see <see cref="IEntityHandle{TId,TEntity}"/> for that.
/// </summary>
/// <typeparam name="TId">The ID type of the entity that this reference represents.</typeparam>
/// <typeparam name="TEntity">The type of the entity that this reference represents.</typeparam>
internal interface IEntityReference<out TId, out TEntity> : IEntityReference<TId>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     Gets the underlying entity, if it is still in memory.
    /// </summary>
    /// <param name="isSuccess">
    ///     Whether the entity was successfully fetched from memory.
    /// </param>
    /// <returns>The underlying entity if found; otherwise <see langword="null"/>.</returns>
    TEntity? GetReference(out bool isSuccess);

    /// <summary>
    ///     Creates a new <see cref="IEntityHandle{TId,TEntity}"/> wrapping the underlying entity, preventing it from
    ///     being garbage collected.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEntityHandle{TId,TEntity}"/> that wraps the entity if found; otherwise
    ///     <see langword="null"/>.
    /// </returns>
    IEntityHandle<TId, TEntity>? AllocateHandle();
}

/// <summary>
///     Represents an entity broker for a given entity and actor type.
/// </summary>
/// <typeparam name="TId">The type of the entities unique identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TActor">The type of the actor for the <typeparamref name="TEntity"/>.</typeparam>
/// <typeparam name="TModel">The model used to construct the given entity.</typeparam>
internal interface IEntityBroker<TId, TEntity, in TActor, TModel> : IEntityBroker<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    /// <summary>
    ///     Gets a handle to an entity, given its identity, from either the reference cache or the underlying
    ///     <see cref="ICacheProvider"/>.
    /// </summary>
    /// <param name="path">The cache path used to construct the entity if needed.</param>
    /// <param name="identity">The identity of the entity to get.</param>
    /// <param name="storeInfo">The store info for the <typeparamref name="TEntity"/>.</param>
    /// <param name="actor">The actor provided to the entity upon construction.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous update operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous get operation. The result of the <see cref="ValueTask{TResult}"/>
    ///     is the entity handle that the <paramref name="identity"/> represents, if found; otherwise
    ///     <see langword="null"/>.
    /// </returns>
    ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        TActor? actor = null,
        CancellationToken token = default
    );

    ValueTask<IEntityHandle<TId, TEntity>?> IEntityBroker<TId, TEntity, TModel>.GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token
    ) => GetAsync(path, identity, storeInfo, null, token);
}

/// <summary>
///     Represents an entity broker for a given entity type.
/// </summary>
/// <typeparam name="TId">The type of the entities unique identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TModel">The model used to construct the given entity.</typeparam>
internal interface IEntityBroker<TId, TEntity, TModel> : IManageableEntityBroker<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    /// <summary>
    ///     Attaches a latent entity to the broker and underlying <see cref="ICacheProvider"/>, ensuring that the entity
    ///     is kept up-to-date with the cache and gateway.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="entity">The latent entity to attach to this broker.</param>
    /// <param name="storeInfo">The store info for the <paramref name="entity"/>.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous attach operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous attach operation.
    /// </returns>
    ValueTask AttachLatentEntityAsync(
        TId id,
        TEntity entity,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token
    );

    /// <summary>
    ///     Attempts to create a handle used to track a latent entity.
    /// </summary>
    /// <param name="model">The model that will be used to construct the latent entity.</param>
    /// <param name="entity">
    ///     If the result is <see langword="false"/>, an entity that represents the given <paramref name="model"/> that
    ///     has been constructed previously, which is attached to this broker and the underlying
    ///     <see cref="ICacheProvider"/>.
    /// </param>
    /// <param name="handle">
    ///     If the result is <see langword="true"/>, the handle used to prevent any future operations for the entity
    ///     that the <paramref name="model"/> represents.
    /// </param>
    /// <param name="token">A cancellation token used to cancel the asynchronous attach operation.</param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="handle"/> was created; otherwise <see langword="false"/> with
    ///     the <paramref name="entity"/> being provided.
    /// </returns>
    bool TryCreateLatentHandle(
        TModel model,
        [MaybeNullWhen(true)] out TEntity entity,
        [MaybeNullWhen(false)] out IDisposable handle,
        CancellationToken token
    );

    /// <summary>
    ///     Updates the underlying <see cref="ICacheProvider"/> with a given model, as well as the entity that's
    ///     in-reference, if it exists.
    /// </summary>
    /// <param name="model">
    ///     The model to update the <see cref="ICacheProvider"/> and <typeparamref name="TEntity"/> with.
    /// </param>
    /// <param name="storeInfo">The store info for the <typeparamref name="TEntity"/>.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous update operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous update operation.
    /// </returns>
    ValueTask UpdateAsync(TModel model, IStoreInfo<TId, TModel> storeInfo, CancellationToken token);

    /// <summary>
    ///     Updates the underlying <see cref="ICacheProvider"/> with the given models, as well as the entities that
    ///     are in-reference, if any exists.
    /// </summary>
    /// <param name="models">
    ///     A collection of models to update the <see cref="ICacheProvider"/> and <typeparamref name="TEntity"/>
    /// </param>
    /// <param name="storeInfo">The store info for the <typeparamref name="TEntity"/>.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous update operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous update operation.
    /// </returns>
    ValueTask BatchUpdateAsync(
        IEnumerable<TModel> models,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token
    );

    /// <summary>
    ///     Creates or updates entities from a collection of models.
    /// </summary>
    /// <param name="path">The cache path used to construct the entities.</param>
    /// <param name="models">A collection of models used to construct the entities.</param>
    /// <param name="store">
    ///     The optional store to update with the given models, doesn't preform a cache update
    ///     if the store is <see langword="null"/>.
    /// </param>
    /// <param name="token">A cancellation token used to cancel the asynchronous update operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous update operation. The result of the
    ///     <see cref="ValueTask{TResult}"/> is a collection of entity handles, in order, for each provided model
    ///     in <paramref name="models"/>.
    /// </returns>
    ValueTask<IEnumerable<IEntityHandle<TId, TEntity>>> BatchCreateOrUpdateAsync(
        CachePathable path,
        IEnumerable<TModel> models,
        IStoreInfo<TId, TModel>? store = null,
        CancellationToken token = default
    );

    /// <summary>
    ///     Gets a handle to an entity, given its identity, from either the reference cache or the underlying
    ///     <see cref="ICacheProvider"/>.
    /// </summary>
    /// <param name="path">The cache path used to construct the entity if needed.</param>
    /// <param name="identity">The identity of the entity to get.</param>
    /// <param name="storeInfo">The store info for the <typeparamref name="TEntity"/>.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous update operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous get operation. The result of the <see cref="ValueTask{TResult}"/>
    ///     is the entity handle that the <paramref name="identity"/> represents, if found; otherwise
    ///     <see langword="null"/>.
    /// </returns>
    ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default
    );

    /// <summary>
    ///     Gets all entities from the underlying <see cref="ICacheProvider"/>.
    /// </summary>
    /// <param name="path">The cache path used to construct the entity if needed.</param>
    /// <param name="storeInfo">The store info for the <typeparamref name="TEntity"/>.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous update operation.</param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}"/> representing the asynchronous get and construction operation. If the
    ///     provided <paramref name="storeInfo"/> contains multiple stores, their pages will be combined in the batched
    ///     <see cref="IReadOnlyCollection{T}"/>.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default);

    IAsyncEnumerable<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> QueryAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        CancellationToken token = default);

    /// <summary>
    ///     Gets all unique identifiers from the underlying <see cref="ICacheProvider"/>.
    /// </summary>
    /// <param name="storeInfo">THe store info for the <typeparamref name="TEntity"/>.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous update operation.</param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}"/> representing the asynchronous get operation. If the  provided
    ///     <paramref name="storeInfo"/> contains multiple stores, their pages will be combined in the batched
    ///     <see cref="IReadOnlyCollection{T}"/>.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<TId>> GetAllIdsAsync(IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default);
}

/// <summary>
///     Represents a manageable entity broker for a particular entity.
/// </summary>
/// <typeparam name="TId">The type of the entities unique identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TModel">The model used to construct the given entity.</typeparam>
internal interface IManageableEntityBroker<TId, in TEntity, in TModel> : IEntityBroker<TId>
    where TId : IEquatable<TId>
    where TEntity : class, ICacheableEntity<TId>, IEntityOf<TModel>
    where TModel : IEntityModel<TId>
{
    /// <summary>
    ///     Releases a handle for an entity that this broker manages.
    /// </summary>
    /// <param name="handle">The handle to release.</param>
    void ReleaseHandle(IEntityHandle<TId, TEntity> handle);

    /// <summary>
    ///     Transfers the construction of an entity to this broker.
    /// </summary>
    /// <param name="model">The model to construct the entity from.</param>
    /// <param name="context">The context to construct the entity with.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous construction operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous construction operation. The result of the
    ///     <see cref="ValueTask{TResult}"/> is a handle pointing to the constructed entity.
    /// </returns>
    /// <remarks>
    ///     There is no guarantee that this function will construct an entity, but a handle will always be returned.
    ///     If the entity that the <paramref name="model"/> will represent exists already, or is being created
    ///     concurrently, the handle returned will point to that entity.
    /// </remarks>
    ValueTask<IEntityHandle<TId>> TransferConstructionOfEntity(
        TModel model,
        IGatewayConstructionContext context,
        CancellationToken token
    );

    /// <summary>
    ///     Updates an entity that's in-reference with a given model. This function does not update
    ///     the underlying <see cref="ICacheProvider"/> with the model.
    /// </summary>
    /// <param name="model">The model to update the in-reference entity with.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous construction operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous update operation.
    /// </returns>
    ValueTask UpdateInReferenceEntityAsync(TModel model, CancellationToken token);

    /// <summary>
    ///     Updates multiple entities that are in-reference with the given models. This function does not update
    ///     the underlying <see cref="ICacheProvider"/> with the models.
    /// </summary>
    /// <param name="models">The models used to update the in-reference entities.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous construction operation.</param>
    /// <returns>
    ///     A value task representing the asynchronous update operation.
    /// </returns>
    ValueTask UpdateInReferenceEntitiesAsync(IEnumerable<TModel> models, CancellationToken token);

    /// <summary>
    ///     Gets the entity type that this broker represents.
    /// </summary>
    Type IEntityBroker.EntityType => typeof(TEntity);
}

/// <summary>
///     Represents a bare-bones entity broker for an entity with the given ID type.
/// </summary>
/// <typeparam name="TId">The type of the entities unique identifier.</typeparam>
internal interface IEntityBroker<in TId> : IEntityBroker;

/// <summary>
///     Represents a bare-bones entity broker.
/// </summary>
internal interface IEntityBroker
{
    /// <summary>
    ///     Gets the type of the entity this broker represents.
    /// </summary>
    Type EntityType { get; }
}
