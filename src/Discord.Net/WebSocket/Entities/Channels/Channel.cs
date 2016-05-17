using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    //TODO: Look into Internal abstract pattern - can we get rid of this?
    public abstract class Channel : IChannel
    {
        /// <inheritdoc />
        public ulong Id { get; private set; }
        public IEnumerable<User> Users => GetUsersInternal();

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);

        internal Channel(ulong id)
        {
            Id = id;
        }

        /// <inheritdoc />
        public User GetUser(ulong id)
            => GetUserInternal(id);

        protected abstract User GetUserInternal(ulong id);
        protected abstract IEnumerable<User> GetUsersInternal();

        Task<IEnumerable<IUser>> IChannel.GetUsers()
            => Task.FromResult<IEnumerable<IUser>>(GetUsersInternal());
        Task<IEnumerable<IUser>> IChannel.GetUsers(int limit, int offset)
            => Task.FromResult<IEnumerable<IUser>>(GetUsersInternal().Skip(offset).Take(limit));
        Task<IUser> IChannel.GetUser(ulong id)
            => Task.FromResult<IUser>(GetUserInternal(id));
    }
}
