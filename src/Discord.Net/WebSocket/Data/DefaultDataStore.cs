using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket.Data
{
    public class DefaultDataStore : IDataStore
    {
        private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
        private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
        private const double CollectionMultiplier = 1.05; //Add buffer to handle growth
        private const double CollectionConcurrencyLevel = 1; //WebSocket updater/event handler. //TODO: Needs profiling, increase to 2?

        private ConcurrentDictionary<ulong, Channel> _channels;
        private ConcurrentDictionary<ulong, Guild> _guilds;
        private ConcurrentDictionary<ulong, Role> _roles;
        private ConcurrentDictionary<ulong, User> _users;

        public IEnumerable<Channel> Channels => _channels.Select(x => x.Value);
        public IEnumerable<Guild> Guilds => _guilds.Select(x => x.Value);
        public IEnumerable<Role> Roles => _roles.Select(x => x.Value);
        public IEnumerable<User> Users => _users.Select(x => x.Value);

        public DefaultDataStore(int guildCount, int dmChannelCount)
        {
            _channels = new ConcurrentDictionary<ulong, Channel>(1, (int)((guildCount * AverageChannelsPerGuild + dmChannelCount) * CollectionMultiplier));
            _guilds = new ConcurrentDictionary<ulong, Guild>(1, (int)(guildCount * CollectionMultiplier));
            _users = new ConcurrentDictionary<ulong, User>(1, (int)(guildCount * AverageUsersPerGuild * CollectionMultiplier));
        }

        public Channel GetChannel(ulong id)
        {
            Channel channel;
            if (_channels.TryGetValue(id, out channel))
                return channel;
            return null;
        }
        public void AddChannel(Channel channel)
        {
            _channels[channel.Id] = channel;
        }
        public Channel RemoveChannel(ulong id)
        {
            Channel channel;
            if (_channels.TryRemove(id, out channel))
                return channel;
            return null;
        }

        public Guild GetGuild(ulong id)
        {
            Guild guild;
            if (_guilds.TryGetValue(id, out guild))
                return guild;
            return null;
        }
        public void AddGuild(Guild guild)
        {
            _guilds[guild.Id] = guild;
        }
        public Guild RemoveGuild(ulong id)
        {
            Guild guild;
            if (_guilds.TryRemove(id, out guild))
                return guild;
            return null;
        }

        public Role GetRole(ulong id)
        {
            Role role;
            if (_roles.TryGetValue(id, out role))
                return role;
            return null;
        }
        public void AddRole(Role role)
        {
            _roles[role.Id] = role;
        }
        public Role RemoveRole(ulong id)
        {
            Role role;
            if (_roles.TryRemove(id, out role))
                return role;
            return null;
        }

        public User GetUser(ulong id)
        {
            User user;
            if (_users.TryGetValue(id, out user))
                return user;
            return null;
        }
        public void AddUser(User user)
        {
            _users[user.Id] = user;
        }
        public User RemoveUser(ulong id)
        {
            User user;
            if (_users.TryRemove(id, out user))
                return user;
            return null;
        }
    }
}
