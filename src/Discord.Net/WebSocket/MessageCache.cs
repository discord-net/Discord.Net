using Discord.API.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class MessageCache
    {
        private readonly DiscordClient _discord;
        private readonly IMessageChannel _channel;
        private readonly ConcurrentDictionary<ulong, Message> _messages;
        private readonly ConcurrentQueue<ulong> _orderedMessages;
        private readonly int _size;

        public IEnumerable<Message> Messages => _messages.Select(x => x.Value);

        public MessageCache(DiscordClient discord, IMessageChannel channel)
        {
            _discord = discord;
            _channel = channel;
            _size = discord.MessageCacheSize;
            _messages = new ConcurrentDictionary<ulong, Message>(1, (int)(_size * 1.05));
            _orderedMessages = new ConcurrentQueue<ulong>();
        }

        internal void Add(Message message)
        {
            if (_messages.TryAdd(message.Id, message))
            {
                _orderedMessages.Enqueue(message.Id);

                ulong msgId;
                Message msg;
                while (_orderedMessages.Count > _size && _orderedMessages.TryDequeue(out msgId))
                    _messages.TryRemove(msgId, out msg);
            }
        }

        internal void Remove(ulong id)
        {
            Message msg;
            _messages.TryRemove(id, out msg);
        }

        public Message Get(ulong id)
        {
            Message result;
            if (_messages.TryGetValue(id, out result))
                return result;
            return null;
        }
        public IImmutableList<Message> GetMany(ulong? fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<Message>.Empty;

            IEnumerable<ulong> cachedMessageIds;
            if (fromMessageId == null)
                cachedMessageIds = _orderedMessages;
            else if (dir == Direction.Before)
                cachedMessageIds = _orderedMessages.Where(x => x < fromMessageId.Value);
            else
                cachedMessageIds = _orderedMessages.Where(x => x > fromMessageId.Value);

            return cachedMessageIds
                .Take(limit)
                .Select(x =>
                {
                    Message msg;
                    if (_messages.TryGetValue(x, out msg))
                        return msg;
                    return null;
                })
                .Where(x => x != null)
                .ToImmutableArray();
        }

        public async Task<IEnumerable<Message>> Download(ulong? fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            //TODO: Test heavily

            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<Message>.Empty;

            var cachedMessages = GetMany(fromMessageId, dir, limit);
            if (cachedMessages.Count == limit)
                return cachedMessages;
            else if (cachedMessages.Count > limit)
                return cachedMessages.Skip(cachedMessages.Count - limit);
            else
            {
                var args = new GetChannelMessagesParams
                {
                    Limit = limit - cachedMessages.Count,
                    RelativeDirection = dir,
                    RelativeMessageId = dir == Direction.Before ? cachedMessages[0].Id : cachedMessages[cachedMessages.Count - 1].Id
                };
                var downloadedMessages = await _discord.ApiClient.GetChannelMessages(_channel.Id, args).ConfigureAwait(false);
                //TODO: Ugly channel cast
                return cachedMessages.AsEnumerable().Concat(downloadedMessages.Select(x => new Message(_channel, (_channel as Channel).GetUser(x.Id), x))).ToImmutableArray();
            }
        }
    }
}
