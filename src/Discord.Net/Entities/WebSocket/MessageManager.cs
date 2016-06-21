using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    internal class MessageManager
    {
        private readonly DiscordSocketClient _discord;
        private readonly ICachedMessageChannel _channel;

        public virtual IReadOnlyCollection<CachedMessage> Messages 
            => ImmutableArray.Create<CachedMessage>();

        public MessageManager(DiscordSocketClient discord, ICachedMessageChannel channel)
        {
            _discord = discord;
            _channel = channel;
        }

        public virtual void Add(CachedMessage message) { }
        public virtual CachedMessage Remove(ulong id) => null;
        public virtual CachedMessage Get(ulong id) => null;

        public virtual IImmutableList<CachedMessage> GetMany(ulong? fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ImmutableArray.Create<CachedMessage>();

        public virtual async Task<CachedMessage> DownloadAsync(ulong id)
        {
            var model = await _discord.ApiClient.GetChannelMessageAsync(_channel.Id, id).ConfigureAwait(false);
            if (model != null)
                return new CachedMessage(_channel, new User(model.Author.Value), model);
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
                Optional<ulong> relativeId;
                if (cachedMessages.Count == 0)
                    relativeId = fromId ?? new Optional<ulong>();
                else
                    relativeId = dir == Direction.Before ? cachedMessages[0].Id : cachedMessages[cachedMessages.Count - 1].Id;
                var args = new GetChannelMessagesParams
                {
                    Limit = limit - cachedMessages.Count,
                    RelativeDirection = dir,
                    RelativeMessageId = relativeId
                };
                var downloadedMessages = await _discord.ApiClient.GetChannelMessagesAsync(_channel.Id, args).ConfigureAwait(false);

                var guild = (_channel as ICachedGuildChannel).Guild;
                return cachedMessages.Concat(downloadedMessages.Select(x =>
                {
                    IUser user = _channel.GetUser(x.Author.Value.Id, true);
                    if (user == null)
                    {
                        var newUser = new User(x.Author.Value);
                        if (guild != null)
                            user = new GuildUser(guild, newUser);
                        else
                            user = newUser;
                    }
                    return new CachedMessage(_channel, user, x);
                })).ToImmutableArray();
            }
        }
    }
}
