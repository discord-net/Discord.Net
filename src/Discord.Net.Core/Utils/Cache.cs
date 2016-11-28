using System;
using System.Reflection;
using System.Threading.Tasks;
using TId = Discord.IEntity<ulong>;


namespace Discord
{
    public abstract class Cache<T> where T : IEntity<ulong> 
    {
        public bool IsCached => Value != null;
        public TId Id { get; }
        public T Value { get; }

        protected Cache(TId id)
        {
            Id = id;
        }

        public async Task<T> DownloadAsync() => typeof(T).GetRuntimeMethod("DownloadAsync",{ulong id});
        public async Task<T> GetOrDownloadAsync() => IsCached ? Value : await DownloadAsync();
    }
}