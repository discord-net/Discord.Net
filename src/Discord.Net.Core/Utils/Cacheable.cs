using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a cached entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that is cached.</typeparam>
    /// <typeparam name="TId">The type of this entity's ID.</typeparam>
    public struct Cacheable<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        ///     Gets whether this entity is cached.
        /// </summary>
        public bool HasValue { get; }
        /// <summary>
        ///     Gets the ID of this entity.
        /// </summary>
        public TId Id { get; }
        /// <summary>
        ///     Gets the entity if it could be pulled from cache.
        /// </summary>
        /// <remarks>
        ///     This value is not guaranteed to be set; in cases where the entity cannot be pulled from cache, it is
        ///     <c>null</c>.
        /// </remarks>
        public TEntity Value { get; }
        private Func<Task<TEntity>> DownloadFunc { get; }

        internal Cacheable(TEntity value, TId id, bool hasValue , Func<Task<TEntity>> downloadFunc)
        {
            Value = value;
            Id = id;
            HasValue = hasValue;
            DownloadFunc = downloadFunc;
        }

        /// <summary>
        ///     Downloads this entity to cache.
        /// </summary>
        /// <exception cref="Discord.Net.HttpException">Thrown when used from a user account.</exception>
        /// <exception cref="NullReferenceException">Thrown when the message is deleted.</exception>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the downloaded entity.
        /// </returns>
        public async Task<TEntity> DownloadAsync()
        {
            return await DownloadFunc().ConfigureAwait(false);
        }

        /// <summary>
        ///     Returns the cached entity if it exists; otherwise downloads it.
        /// </summary>
        /// <exception cref="Discord.Net.HttpException">Thrown when used from a user account.</exception>
        /// <exception cref="NullReferenceException">Thrown when the message is deleted and is not in cache.</exception>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a cached or downloaded entity.
        /// </returns>
        public async Task<TEntity> GetOrDownloadAsync() => HasValue ? Value : await DownloadAsync().ConfigureAwait(false);
    }
}
