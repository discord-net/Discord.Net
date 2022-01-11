using System;
using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a collection of <see cref="IEqualityComparer{T}"/> for various Discord objects.
    /// </summary>
    public static class DiscordComparers
    {
        /// <summary>
        ///     Gets an <see cref="IEqualityComparer{T}"/> to be used to compare users.
        /// </summary>
        public static IEqualityComparer<IUser> UserComparer => _userComparer ??= new EntityEqualityComparer<IUser, ulong>();
        /// <summary>
        ///     Gets an <see cref="IEqualityComparer{T}"/> to be used to compare guilds.
        /// </summary>
        public static IEqualityComparer<IGuild> GuildComparer => _guildComparer ??= new EntityEqualityComparer<IGuild, ulong>();
        /// <summary>
        ///     Gets an <see cref="IEqualityComparer{T}"/> to be used to compare channels.
        /// </summary>
        public static IEqualityComparer<IChannel> ChannelComparer => _channelComparer ??= new EntityEqualityComparer<IChannel, ulong>();
        /// <summary>
        ///     Gets an <see cref="IEqualityComparer{T}"/> to be used to compare roles.
        /// </summary>
        public static IEqualityComparer<IRole> RoleComparer => _roleComparer ??= new EntityEqualityComparer<IRole, ulong>();
        /// <summary>
        ///     Gets an <see cref="IEqualityComparer{T}"/> to be used to compare messages.
        /// </summary>
        public static IEqualityComparer<IMessage> MessageComparer => _messageComparer ??= new EntityEqualityComparer<IMessage, ulong>();

        private static IEqualityComparer<IUser> _userComparer;
        private static IEqualityComparer<IGuild> _guildComparer;
        private static IEqualityComparer<IChannel> _channelComparer;
        private static IEqualityComparer<IRole> _roleComparer;
        private static IEqualityComparer<IMessage> _messageComparer;

        private sealed class EntityEqualityComparer<TEntity, TId> : EqualityComparer<TEntity>
            where TEntity : IEntity<TId>
            where TId : IEquatable<TId>
        {
            public override bool Equals(TEntity x, TEntity y)
            {
                return (x, y) switch
                {
                    (null, null) => true,
                    (null, _)    => false,
                    (_, null)    => false,
                    var (l, r)   => l.Id.Equals(r.Id)
                };
            }

            public override int GetHashCode(TEntity obj)
            {
                return obj?.Id.GetHashCode() ?? 0;
            }
        }
    }
}
