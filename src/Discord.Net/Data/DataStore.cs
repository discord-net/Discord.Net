using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Discord
{
    public class DataStore
    {
        private const int CollectionConcurrencyLevel = 1; //WebSocket updater/event handler. //TODO: Needs profiling, increase to 2?
        private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
        private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
        private const double CollectionMultiplier = 1.05; //Add 5% buffer to handle growth

        private readonly ConcurrentDictionary<ulong, ICachedChannel> _channels;
        private readonly ConcurrentDictionary<ulong, CachedDMChannel> _dmChannels;
        private readonly ConcurrentDictionary<ulong, CachedGuild> _guilds;
        private readonly ConcurrentDictionary<ulong, CachedGlobalUser> _users;

        internal IReadOnlyCollection<ICachedChannel> Channels => _channels.ToReadOnlyCollection();
        internal IReadOnlyCollection<CachedDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();
        internal IReadOnlyCollection<CachedGuild> Guilds => _guilds.ToReadOnlyCollection();
        internal IReadOnlyCollection<CachedGlobalUser> Users => _users.ToReadOnlyCollection();

        public DataStore(int guildCount, int dmChannelCount)
        {
            double estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
            double estimatedUsersCount = guildCount * AverageUsersPerGuild;
            _channels = new ConcurrentDictionary<ulong, ICachedChannel>(CollectionConcurrencyLevel, (int)(estimatedChannelCount * CollectionMultiplier));
            _dmChannels = new ConcurrentDictionary<ulong, CachedDMChannel>(CollectionConcurrencyLevel, (int)(dmChannelCount * CollectionMultiplier));
            _guilds = new ConcurrentDictionary<ulong, CachedGuild>(CollectionConcurrencyLevel, (int)(guildCount * CollectionMultiplier));
            _users = new ConcurrentDictionary<ulong, CachedGlobalUser>(CollectionConcurrencyLevel, (int)(estimatedUsersCount * CollectionMultiplier));
        }

        internal ICachedChannel GetChannel(ulong id)
        {
            ICachedChannel channel;
            if (_channels.TryGetValue(id, out channel))
                return channel;
            return null;
        }
        internal void AddChannel(ICachedChannel channel)
        {
            _channels[channel.Id] = channel;
        }
        internal ICachedChannel RemoveChannel(ulong id)
        {
            ICachedChannel channel;
            if (_channels.TryRemove(id, out channel))
                return channel;
            return null;
        }

        internal CachedDMChannel GetDMChannel(ulong userId)
        {
            CachedDMChannel channel;
            if (_dmChannels.TryGetValue(userId, out channel))
                return channel;
            return null;
        }
        internal void AddDMChannel(CachedDMChannel channel)
        {
            _channels[channel.Id] = channel;
            _dmChannels[channel.Recipient.Id] = channel;
        }
        internal CachedDMChannel RemoveDMChannel(ulong userId)
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

        internal CachedGuild GetGuild(ulong id)
        {
            CachedGuild guild;
            if (_guilds.TryGetValue(id, out guild))
                return guild;
            return null;
        }
        internal void AddGuild(CachedGuild guild)
        {
            _guilds[guild.Id] = guild;
        }
        internal CachedGuild RemoveGuild(ulong id)
        {
            CachedGuild guild;
            if (_guilds.TryRemove(id, out guild))
                return guild;
            return null;
        }

        internal CachedGlobalUser GetUser(ulong id)
        {
            CachedGlobalUser user;
            if (_users.TryGetValue(id, out user))
                return user;
            return null;
        }
        internal CachedGlobalUser GetOrAddUser(ulong id, Func<ulong, CachedGlobalUser> userFactory)
        {
            return _users.GetOrAdd(id, userFactory);
        }
        internal CachedGlobalUser RemoveUser(ulong id)
        {
            CachedGlobalUser user;
            if (_users.TryRemove(id, out user))
                return user;
            return null;
        }
    }
}
