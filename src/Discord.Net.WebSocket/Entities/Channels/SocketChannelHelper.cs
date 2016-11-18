using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.WebSocket
{
    internal static class SocketChannelHelper
    {
        public static IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ISocketMessageChannel channel, DiscordSocketClient discord, MessageCache messages,
            ulong? fromMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
        {
            if (dir == Direction.Around)
                throw new NotImplementedException(); //TODO: Impl

            IReadOnlyCollection<SocketMessage> cachedMessages = null;
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> result = null;
            
            if (dir == Direction.After && fromMessageId == null)
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

            if (dir == Direction.Before || mode == CacheMode.CacheOnly)
            {
                if (messages != null) //Cache enabled
                    cachedMessages = messages.GetMany(fromMessageId, dir, limit);
                else
                    cachedMessages = ImmutableArray.Create<SocketMessage>();
                result = ImmutableArray.Create(cachedMessages).ToAsyncEnumerable<IReadOnlyCollection<IMessage>>();
            }

            if (dir == Direction.Before)
            {
                limit -= cachedMessages.Count;
                if (mode == CacheMode.CacheOnly || limit <= 0)
                    return result;

                //Download remaining messages
                ulong? minId = cachedMessages.Count > 0 ? cachedMessages.Min(x => x.Id) : fromMessageId;
                var downloadedMessages = ChannelHelper.GetMessagesAsync(channel, discord, minId, dir, limit, options);
                return result.Concat(downloadedMessages);
            }
            else
            {
                if (mode == CacheMode.CacheOnly)
                    return result;

                //Dont use cache in this case
                return ChannelHelper.GetMessagesAsync(channel, discord, fromMessageId, dir, limit, options);
            }
        }
        public static IReadOnlyCollection<SocketMessage> GetCachedMessages(SocketChannel channel, DiscordSocketClient discord, MessageCache messages,
            ulong? fromMessageId, Direction dir, int limit)
        {
            if (messages != null) //Cache enabled
                return messages.GetMany(fromMessageId, dir, limit);
            else
                return ImmutableArray.Create<SocketMessage>();
        }

        public static void AddMessage(ISocketMessageChannel channel, DiscordSocketClient discord,
            SocketMessage msg)
        {
            //TODO: C#7 Candidate for pattern matching
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
            //TODO: C#7 Candidate for pattern matching
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
