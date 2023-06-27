using Discord.WebSocket.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal static class EntitySourceExtensions
    {
        public static IEntitySource<TId, TNew> Transform<TId, TOld, TNew>(this IEntitySource<TId, TOld> old)
            where TId : IEquatable<TId>
            where TNew : class, IEntity<TId>, TOld
            where TOld : class, IEntity<TId>
            => new TransitiveEntitySource<TId, TNew, TOld>(old);

        public static IEntitySource<ulong, TChannel> TransformChannel<TChannel>(this IEntitySource<ulong, SocketChannel> old)
            where TChannel : SocketChannel
            => Transform<ulong, SocketChannel, TChannel>(old);
    }
}
