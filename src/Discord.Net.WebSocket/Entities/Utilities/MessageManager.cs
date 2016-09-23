using Discord.API.Rest;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    internal class MessageManager
    {
        private readonly DiscordSocketClient _discord;
        private readonly ISocketMessageChannel _channel;

        public virtual IReadOnlyCollection<ISocketMessage> Messages
            => ImmutableArray.Create<ISocketMessage>();

        public MessageManager(DiscordSocketClient discord, ISocketMessageChannel channel)
        {
            _discord = discord;
            _channel = channel;
        }

        public virtual void Add(ISocketMessage message) { }
        public virtual ISocketMessage Remove(ulong id) => null;
        public virtual ISocketMessage Get(ulong id) => null;

        public virtual IImmutableList<ISocketMessage> GetMany(ulong? fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ImmutableArray.Create<ISocketMessage>();

        public virtual async Task<ISocketMessage> DownloadAsync(ulong id)
        {
            var model = await _discord.ApiClient.GetChannelMessageAsync(_channel.Id, id).ConfigureAwait(false);
            if (model != null)
                return Create(new User(model.Author.Value), model);
            return null;
        }
        public async Task<IReadOnlyCollection<ISocketMessage>> DownloadAsync(ulong? fromId, Direction dir, int limit)
        {
            //TODO: Test heavily, especially the ordering of messages
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<ISocketMessage>.Empty;

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
                    RelativeDirection = dir
                };
                if (cachedMessages.Count == 0)
                {
                    if (fromId != null)
                        args.RelativeMessageId = fromId.Value;
                }
                else
                    args.RelativeMessageId = dir == Direction.Before ? cachedMessages[0].Id : cachedMessages[cachedMessages.Count - 1].Id;
                var downloadedMessages = await _discord.ApiClient.GetChannelMessagesAsync(_channel.Id, args).ConfigureAwait(false);

                var guild = (_channel as ISocketGuildChannel)?.Guild;
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
                    return Create(user, x);
                })).ToImmutableArray();
            }
        }

        public ISocketMessage Create(IUser author, Model model)
        {
            if (model.Type == MessageType.Default)
                return new SocketUserMessage(_channel, author, model);
            else
                return new SocketSystemMessage(_channel, author, model);
        }
    }
}
