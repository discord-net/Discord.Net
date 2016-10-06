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

        public IReadOnlyCollection<SocketMessage> Messages => _messages.ToReadOnlyCollection();

        public MessageCache(DiscordSocketClient discord, IChannel channel)
        {
            _size = discord.MessageCacheSize;
            _messages = new ConcurrentDictionary<ulong, SocketMessage>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(_size * 1.05));
            _orderedMessages = new ConcurrentQueue<ulong>();
        }

        public void Add(SocketMessage message)
        {
            if (_messages.TryAdd(message.Id, message))
            {
                _orderedMessages.Enqueue(message.Id);

                ulong msgId;
                SocketMessage msg;
                while (_orderedMessages.Count > _size && _orderedMessages.TryDequeue(out msgId))
                    _messages.TryRemove(msgId, out msg);
            }
        }

        public SocketMessage Remove(ulong id)
        {
            SocketMessage msg;
            _messages.TryRemove(id, out msg);
            return msg;
        }

        public SocketMessage Get(ulong id)
        {
            SocketMessage result;
            if (_messages.TryGetValue(id, out result))
                return result;
            return null;
        }
        public IReadOnlyCollection<SocketMessage> GetMany(ulong? fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
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
                .Select(x =>
                {
                    SocketMessage msg;
                    if (_messages.TryGetValue(x, out msg))
                        return msg;
                    return null;
                })
                .Where(x => x != null)
                .Take(limit)
                .ToImmutableArray();
        }
    }
}
