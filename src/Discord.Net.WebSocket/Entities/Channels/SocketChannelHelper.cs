using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal static class SocketChannelHelper
    {
        public static async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(SocketChannel channel, DiscordSocketClient discord, MessageCache messages, 
            ulong? fromMessageId, Direction dir, int limit)
        {
            if (messages == null) //Cache disabled
            {
                var msgs = await ChannelHelper.GetMessagesAsync(channel, discord, fromMessageId, dir, limit).Flatten();
                return msgs.ToImmutableArray();
            }

            var cachedMessages = messages.GetMany(fromMessageId, dir, limit);
            limit -= cachedMessages.Count;
            if (limit == 0)
                return cachedMessages;
            
            if (dir == Direction.Before)
                fromMessageId = cachedMessages.Min(x => x.Id);
            else
                fromMessageId = cachedMessages.Max(x => x.Id);
            var downloadedMessages = await ChannelHelper.GetMessagesAsync(channel, discord, fromMessageId, dir, limit).Flatten();
            return cachedMessages.Concat<IMessage>(downloadedMessages).ToImmutableArray();
        }

        public static IAsyncEnumerable<IReadOnlyCollection<IMessage>> PagedGetMessagesAsync(SocketChannel channel, DiscordSocketClient discord, MessageCache messages,
            ulong? fromMessageId, Direction dir, int limit)
        {
            if (messages == null) //Cache disabled
                return ChannelHelper.GetMessagesAsync(channel, discord, fromMessageId, dir, limit);

            var cachedMessages = messages.GetMany(fromMessageId, dir, limit);
            var result = ImmutableArray.Create(cachedMessages).ToAsyncEnumerable<IReadOnlyCollection<IMessage>>();
            limit -= cachedMessages.Count;
            if (limit == 0)
                return result;
            
            if (dir == Direction.Before)
                fromMessageId = cachedMessages.Min(x => x.Id);
            else
                fromMessageId = cachedMessages.Max(x => x.Id);
            var downloadedMessages = ChannelHelper.GetMessagesAsync(channel, discord, fromMessageId, dir, limit);
            return result.Concat(downloadedMessages);
        }

        public static void AddMessage(ISocketMessageChannel channel, DiscordSocketClient discord,
            SocketMessage msg)
        {
            //C#7 Candidate for pattern matching
            if (channel is SocketDMChannel)
                (channel as SocketDMChannel).AddMessage(msg);
            else if (channel is SocketGroupChannel)
                (channel as SocketGroupChannel).AddMessage(msg);
            else if (channel is SocketTextChannel)
                (channel as SocketTextChannel).AddMessage(msg);
            else
                throw new NotSupportedException("Unexpected ISocketMessageChannel type");
        }
        public static SocketMessage RemoveMessage(ISocketMessageChannel channel, DiscordSocketClient discord,
            ulong id)
        {
            //C#7 Candidate for pattern matching
            if (channel is SocketDMChannel)
                return (channel as SocketDMChannel).RemoveMessage(id);
            else if (channel is SocketGroupChannel)
                return (channel as SocketGroupChannel).RemoveMessage(id);
            else if (channel is SocketTextChannel)
                return (channel as SocketTextChannel).RemoveMessage(id);
            else
                throw new NotSupportedException("Unexpected ISocketMessageChannel type");
        }
    }
}
