using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.WebSocket
{
    internal class MessageCache
    {
        private readonly ConcurrentDictionary<ulong, SocketMessage> _messages;
        private readonly ConcurrentQueue<ulong> _orderedMessages;
        private readonly int _size;

        public MessageCache(DiscordSocketClient discord, IChannel channel)
        {
            _size = discord.MessageCacheSize;
            _messages = new ConcurrentDictionary<ulong, SocketMessage>(ConcurrentHashSet.DefaultConcurrencyLevel,
                (int)(_size * 1.05));
            _orderedMessages = new ConcurrentQueue<ulong>();
        }

        public IReadOnlyCollection<SocketMessage> Messages => _messages.ToReadOnlyCollection();

        public void Add(SocketMessage message)
        {
            if (!_messages.TryAdd(message.Id, message)) return;
            _orderedMessages.Enqueue(message.Id);

            while (_orderedMessages.Count > _size && _orderedMessages.TryDequeue(out var msgId))
                _messages.TryRemove(msgId, out _);
        }

        public SocketMessage Remove(ulong id)
        {
            _messages.TryRemove(id, out var msg);
            return msg;
        }

        public SocketMessage Get(ulong id) => _messages.TryGetValue(id, out var result) ? result : null;

        public IReadOnlyCollection<SocketMessage> GetMany(ulong? fromMessageId, Direction dir,
            int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<SocketMessage>.Empty;

            IEnumerable<ulong> cachedMessageIds;
            if (fromMessageId == null)
                cachedMessageIds = _orderedMessages;
            else if (dir == Direction.Before)
                cachedMessageIds = _orderedMessages.Where(x => x < fromMessageId.Value);
            else
                cachedMessageIds = _orderedMessages.Where(x => x > fromMessageId.Value);

            if (dir == Direction.Before)
                cachedMessageIds = cachedMessageIds.Reverse();

            return cachedMessageIds
                .Select(x => { return _messages.TryGetValue(x, out var msg) ? msg : null; })
                .Where(x => x != null)
                .Take(limit)
                .ToImmutableArray();
        }
    }
}
