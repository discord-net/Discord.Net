using System;
namespace Discord
{
    /// <summary>
    ///     Represents a wrapper of a relation between 2 entities, this
    ///     provides an implementation-agnostic way to lazily represent
    ///     a relation.
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the source.</typeparam>
    /// <typeparam name="TId">The ID type of the source.</typeparam>
    public interface IEntitySource<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        ///     Gets the unique identifier of the entity being represented.
        /// </summary>
        TId Id { get; }

        /// <summary>
        ///     Gets the entity if present; otherwise <see langword="null"/>.
        /// </summary>
        TEntity? Value { get; }

        /// <summary>
        ///     Loads the entity from an asynchronous source. This method
        ///     can fetch from the Discord API or a cache - depending on
        ///     configuration and implementation.
        /// </summary>
        /// <param name="options">Request options to use when making an API request.</param>
        /// <param name="token">A cancellation token used to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="ValueTask"/> representing the asynchronous operation of loading
        ///     the entity. The result of the <see cref="ValueTask"/> is the entity OR <see langword="null"/> if
        ///     the entity could not be loaded.
        /// </returns>
        ValueTask<TEntity?> LoadAsync(RequestOptions? options = null, CancellationToken token = default);
    }
}

