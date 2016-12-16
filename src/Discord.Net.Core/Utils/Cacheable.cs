using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public struct Cacheable<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        public bool HasValue => !EqualityComparer<TEntity>.Default.Equals(Value, default(TEntity));
        public ulong Id { get; }
        public TEntity Value { get; }
        private Func<Task<TEntity>> DownloadFunc { get; }

        internal Cacheable(TEntity value, ulong id, Func<Task<TEntity>> downloadFunc)
        {
            Value = value;
            Id = id;
            DownloadFunc = downloadFunc;
        }

        public async Task<TEntity> DownloadAsync()
        {
            return await DownloadFunc();
        }

        public async Task<TEntity> GetOrDownloadAsync() => HasValue ? Value : await DownloadAsync();
    }
}