using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// Contains an entity that may be cached.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that is cached</typeparam>
    /// <typeparam name="TId">The type of this entity's ID</typeparam>
    public struct Cacheable<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Is this entity cached?
        /// </summary>
        public bool HasValue { get; }
        /// <summary>
        /// The ID of this entity.
        /// </summary>
        public TId Id { get; }
        /// <summary>
        /// The entity, if it could be pulled from cache.
        /// </summary>
        /// <remarks>
        /// This value is not guaranteed to be set; in cases where the entity cannot be pulled from cache, it is null.
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
        /// Downloads this entity to cache.
        /// </summary>
        /// <returns>An awaitable Task containing the downloaded entity.</returns>
        /// <exception cref="Discord.Net.HttpException">Thrown when used from a user account.</exception>
        /// <exception cref="NullReferenceException">Thrown when the message is deleted.</exception>
        public async Task<TEntity> DownloadAsync()
        {
            return await DownloadFunc();
        }

        /// <summary>
        /// Returns the cached entity if it exists; otherwise downloads it.
        /// </summary>
        /// <returns>An awaitable Task containing a cached or downloaded entity.</returns>
        /// <exception cref="Discord.Net.HttpException">Thrown when used from a user account.</exception>
        /// <exception cref="NullReferenceException">Thrown when the message is deleted and is not in cache.</exception>
        public async Task<TEntity> GetOrDownloadAsync() => HasValue ? Value : await DownloadAsync();
    }
}