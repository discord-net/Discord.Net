using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Data
{
    public class DefaultDataStore : DataStore
    {
        private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
        private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
        private const double CollectionMultiplier = 1.05; //Add buffer to handle growth
        private const double CollectionConcurrencyLevel = 1; //WebSocket updater/event handler. //TODO: Needs profiling, increase to 2?

        private readonly ConcurrentDictionary<ulong, ICachedChannel> _channels;
        private readonly ConcurrentDictionary<ulong, CachedGuild> _guilds;
        private readonly ConcurrentDictionary<ulong, CachedPublicUser> _users;

        internal override IReadOnlyCollection<ICachedChannel> Channels => _channels.ToReadOnlyCollection();
        internal override IReadOnlyCollection<CachedGuild> Guilds => _guilds.ToReadOnlyCollection();
        internal override IReadOnlyCollection<CachedPublicUser> Users => _users.ToReadOnlyCollection();

        public DefaultDataStore(int guildCount, int dmChannelCount)
        {
            double estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
            double estimatedUsersCount = guildCount * AverageUsersPerGuild;
            _channels = new ConcurrentDictionary<ulong, ICachedChannel>(1, (int)(estimatedChannelCount * CollectionMultiplier));
            _guilds = new ConcurrentDictionary<ulong, CachedGuild>(1, (int)(guildCount * CollectionMultiplier));
            _users = new ConcurrentDictionary<ulong, CachedPublicUser>(1, (int)(estimatedUsersCount * CollectionMultiplier));
        }

        internal override ICachedChannel GetChannel(ulong id)
        {
            ICachedChannel channel;
            if (_channels.TryGetValue(id, out channel))
                return channel;
            return null;
        }
        internal override void AddChannel(ICachedChannel channel)
        {
            _channels[channel.Id] = channel;
        }
        internal override ICachedChannel RemoveChannel(ulong id)
        {
            ICachedChannel channel;
            if (_channels.TryRemove(id, out channel))
                return channel;
            return null;
        }

        internal override CachedGuild GetGuild(ulong id)
        {
            CachedGuild guild;
            if (_guilds.TryGetValue(id, out guild))
                return guild;
            return null;
        }
        internal override void AddGuild(CachedGuild guild)
        {
            _guilds[guild.Id] = guild;
        }
        internal override CachedGuild RemoveGuild(ulong id)
        {
            CachedGuild guild;
            if (_guilds.TryRemove(id, out guild))
                return guild;
            return null;
        }

        internal override CachedPublicUser GetUser(ulong id)
        {
            CachedPublicUser user;
            if (_users.TryGetValue(id, out user))
                return user;
            return null;
        }
        internal override CachedPublicUser GetOrAddUser(ulong id, Func<ulong, CachedPublicUser> userFactory)
        {
            return _users.GetOrAdd(id, userFactory);
        }
        internal override CachedPublicUser RemoveUser(ulong id)
        {
            CachedPublicUser user;
            if (_users.TryRemove(id, out user))
                return user;
            return null;
        }
    }
}
