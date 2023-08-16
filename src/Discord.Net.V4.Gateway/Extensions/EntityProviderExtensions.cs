using Discord.Gateway.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    internal static class EntitySourceExtensions
    {
        public static IEntityProvider<TId, TNew> Transform<TId, TOld, TNew>(this IEntityProvider<TId, TOld> old)
            where TId : IEquatable<TId>
            where TNew : class, IEntity<TId>, TOld
            where TOld : class, IEntity<TId>
            => TransitiveEntityProvider<TId, TNew, TOld>.Create<TId, TNew, TOld>(old);

        public static IEntityProvider<TId, TNew> Transform<TId, TOld, TNew>(this IEntityProvider<TId, TOld> old, Func<TOld, TNew> castFunc)
            where TId : IEquatable<TId>
            where TNew : class, IEntity<TId>
            where TOld : class, IEntity<TId>
            => TransitiveEntityProvider<TId, TNew, TOld>.Create(old, castFunc);

        public static IEntityProvider<ulong, TChannel> TransformChannel<TOld, TChannel>(this IEntityProvider<ulong, TOld> old)
            where TChannel : class, TOld, IChannel
            where TOld : class, IChannel
            => Transform<ulong, TOld, TChannel>(old);
    }
}
