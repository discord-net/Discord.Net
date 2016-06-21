using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Discord.Data
{
    public class DefaultDataStore : DataStore
    {
        private const int CollectionConcurrencyLevel = 1; //WebSocket updater/event handler. //TODO: Needs profiling, increase to 2?
        private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
        private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
        private const double CollectionMultiplier = 1.05; //Add 5% buffer to handle growth

        private readonly ConcurrentDictionary<ulong, ICachedChannel> _channels;
        private readonly ConcurrentDictionary<ulong, CachedDMChannel> _dmChannels;
        private readonly ConcurrentDictionary<ulong, CachedGuild> _guilds;
        private readonly ConcurrentDictionary<ulong, CachedGlobalUser> _users;

        internal override IReadOnlyCollection<ICachedChannel> Channels => _channels.ToReadOnlyCollection();
        internal override IReadOnlyCollection<CachedDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();
        internal override IReadOnlyCollection<CachedGuild> Guilds => _guilds.ToReadOnlyCollection();
        internal override IReadOnlyCollection<CachedGlobalUser> Users => _users.ToReadOnlyCollection();

        public DefaultDataStore(int guildCount, int dmChannelCount)
        {
            double estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
            double estimatedUsersCount = guildCount * AverageUsersPerGuild;
            _channels = new ConcurrentDictionary<ulong, ICachedChannel>(CollectionConcurrencyLevel, (int)(estimatedChannelCount * CollectionMultiplier));
            _dmChannels = new ConcurrentDictionary<ulong, CachedDMChannel>(CollectionConcurrencyLevel, (int)(dmChannelCount * CollectionMultiplier));
            _guilds = new ConcurrentDictionary<ulong, CachedGuild>(CollectionConcurrencyLevel, (int)(guildCount * CollectionMultiplier));
            _users = new ConcurrentDictionary<ulong, CachedGlobalUser>(CollectionConcurrencyLevel, (int)(estimatedUsersCount * CollectionMultiplier));
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

        internal override CachedDMChannel GetDMChannel(ulong userId)
        {
            CachedDMChannel channel;
            if (_dmChannels.TryGetValue(userId, out channel))
                return channel;
            return null;
        }
        internal override void AddDMChannel(CachedDMChannel channel)
        {
            _channels[channel.Id] = channel;
            _dmChannels[channel.Recipient.Id] = channel;
        }
        internal override CachedDMChannel RemoveDMChannel(ulong userId)
        {
            CachedDMChannel channel;
            ICachedChannel ignored;
            if (_dmChannels.TryRemove(userId, out channel))
            {
                if (_channels.TryRemove(channel.Id, out ignored))
                    return channel;
            }
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

        internal override CachedGlobalUser GetUser(ulong id)
        {
            CachedGlobalUser user;
            if (_users.TryGetValue(id, out user))
                return user;
            return null;
        }
        internal override CachedGlobalUser GetOrAddUser(ulong id, Func<ulong, CachedGlobalUser> userFactory)
        {
            return _users.GetOrAdd(id, userFactory);
        }
        internal override CachedGlobalUser RemoveUser(ulong id)
        {
            CachedGlobalUser user;
            if (_users.TryRemove(id, out user))
                return user;
            return null;
        }
    }
}
