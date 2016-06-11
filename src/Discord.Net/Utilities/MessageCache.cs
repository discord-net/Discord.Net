using Discord.API.Rest;
using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    internal class MessageCache
    {
        private readonly DiscordSocketClient _discord;
        private readonly ICachedMessageChannel _channel;
        private readonly ConcurrentDictionary<ulong, CachedMessage> _messages;
        private readonly ConcurrentQueue<ulong> _orderedMessages;
        private readonly int _size;

        public IReadOnlyCollection<CachedMessage> Messages => _messages.ToReadOnlyCollection();

        public MessageCache(DiscordSocketClient discord, ICachedMessageChannel channel)
        {
            _discord = discord;
            _channel = channel;
            _size = discord.MessageCacheSize;
            _messages = new ConcurrentDictionary<ulong, CachedMessage>(1, (int)(_size * 1.05));
            _orderedMessages = new ConcurrentQueue<ulong>();
        }

        public void Add(CachedMessage message)
        {
            if (_messages.TryAdd(message.Id, message))
            {
                _orderedMessages.Enqueue(message.Id);

                ulong msgId;
                CachedMessage msg;
                while (_orderedMessages.Count > _size && _orderedMessages.TryDequeue(out msgId))
                    _messages.TryRemove(msgId, out msg);
            }
        }

        public CachedMessage Remove(ulong id)
        {
            CachedMessage msg;
            _messages.TryRemove(id, out msg);
            return msg;
        }

        public CachedMessage Get(ulong id)
        {
            CachedMessage result;
            if (_messages.TryGetValue(id, out result))
                return result;
            return null;
        }
        public IImmutableList<CachedMessage> GetMany(ulong? fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<CachedMessage>.Empty;

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
                    CachedMessage msg;
                    if (_messages.TryGetValue(x, out msg))
                        return msg;
                    return null;
                })
                .Where(x => x != null)
                .ToImmutableArray();
        }

        public async Task<CachedMessage> DownloadAsync(ulong id)
        {
            var msg = Get(id);
            if (msg != null)
                return msg;
            var model = await _discord.ApiClient.GetChannelMessageAsync(_channel.Id, id).ConfigureAwait(false);
            if (model != null)
                return new CachedMessage(_channel, new User(_discord, model.Author), model);
            return null;
        }
        public async Task<IReadOnlyCollection<CachedMessage>> DownloadAsync(ulong? fromId, Direction dir, int limit)
        {
            //TODO: Test heavily, especially the ordering of messages
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<CachedMessage>.Empty;

            var cachedMessages = GetMany(fromId, dir, limit);
            if (cachedMessages.Count == limit)
                return cachedMessages;
            else if (cachedMessages.Count > limit)
                return cachedMessages.Skip(cachedMessages.Count - limit).ToImmutableArray();
            else
            {
                var args = new GetChannelMessagesParams
                {
                    Limit = limit - cachedMessages.Count,
                    RelativeDirection = dir,
                    RelativeMessageId = dir == Direction.Before ? cachedMessages[0].Id : cachedMessages[cachedMessages.Count - 1].Id
                };
                var downloadedMessages = await _discord.ApiClient.GetChannelMessagesAsync(_channel.Id, args).ConfigureAwait(false);
                return cachedMessages.Concat(downloadedMessages.Select(x => new CachedMessage(_channel, _channel.GetUser(x.Id), x))).ToImmutableArray();
            }
        }
    }
}
