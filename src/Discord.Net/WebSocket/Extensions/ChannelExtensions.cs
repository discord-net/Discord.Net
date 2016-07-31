using System;
using System.Collections.Generic;

namespace Discord.WebSocket.Extensions
{
    public static class ChannelExtensions
    {
        public static IUser GetUser(this IDMChannel channel, ulong id)
            => GetSocketDMChannel(channel).GetUser(id);

        public static IEnumerable<IUser> GetUsers(this IDMChannel channel)
            => GetSocketDMChannel(channel).Users;

        public static IUser GetUser(this IGroupChannel channel, ulong id)
            => GetSocketGroupChannel(channel).GetUser(id);

        public static IEnumerable<IUser> GetUsers(this IGroupChannel channel)
            => GetSocketGroupChannel(channel).Users;

        public static IGuildUser GetUser(this ITextChannel channel, ulong id)
            => GetSocketTextChannel(channel).GetUser(id);

        public static IEnumerable<IGuildUser> GetUsers(this ITextChannel channel)
            => GetSocketTextChannel(channel).Members;

        public static IGuildUser GetUser(this IVoiceChannel channel, ulong id)
            => GetSocketVoiceChannel(channel).GetUser(id);

        public static IEnumerable<IGuildUser> GetUsers(this IVoiceChannel channel)
            => GetSocketVoiceChannel(channel).Members;

        internal static SocketDMChannel GetSocketDMChannel(IDMChannel channel)
        {
            var socketChannel = channel as SocketDMChannel;
            if (socketChannel == null)
                throw new InvalidOperationException("This extension method is only valid on WebSocket Entities");
            return socketChannel;
        }
        internal static SocketGroupChannel GetSocketGroupChannel(IGroupChannel channel)
        {
            var socketChannel = channel as SocketGroupChannel;
            if (socketChannel == null)
                throw new InvalidOperationException("This extension method is only valid on WebSocket Entities");
            return socketChannel;
        }
        internal static SocketTextChannel GetSocketTextChannel(ITextChannel channel)
        {
            var socketChannel = channel as SocketTextChannel;
            if (socketChannel == null)
                throw new InvalidOperationException("This extension method is only valid on WebSocket Entities");
            return socketChannel;
        }
        internal static SocketVoiceChannel GetSocketVoiceChannel(IVoiceChannel channel)
        {
            var socketChannel = channel as SocketVoiceChannel;
            if (socketChannel == null)
                throw new InvalidOperationException("This extension method is only valid on WebSocket Entities");
            return socketChannel;
        }
    }
}
