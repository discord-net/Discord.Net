using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public struct Cached<T> where T : IMessage
    {
        public bool IsCached => !EqualityComparer<T>.Default.Equals(Value, default(T));
        public bool IsDownloadable { get; }
        public ulong Id { get; }
        public T Value { get; }
        public ISocketMessageChannel Channel { get; }

        public Cached(ulong id, T value, ISocketMessageChannel channel, bool isDownloadable = true)
        {
            Id = id;
            Value = value;
            Channel = channel;
            IsDownloadable = isDownloadable;
        }

        public async Task<T> DownloadAsync()
        {
            if (IsDownloadable)
                return (T) await Channel.GetMessageAsync(Id);
            throw new InvalidOperationException("This message cannot be downloaded.");
        }

        public async Task<T> GetOrDownloadAsync() => IsCached ? Value : await DownloadAsync();
    }
}
