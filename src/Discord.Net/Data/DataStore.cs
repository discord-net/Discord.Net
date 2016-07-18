using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
        private readonly ConcurrentHashSet<ulong> _groupChannels;

        internal IReadOnlyCollection<ICachedChannel> Channels => _channels.ToReadOnlyCollection();
        internal IReadOnlyCollection<CachedDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();
        internal IReadOnlyCollection<CachedGroupChannel> GroupChannels => _groupChannels.Select(x => GetChannel(x) as CachedGroupChannel).ToReadOnlyCollection(_groupChannels);
        internal IReadOnlyCollection<CachedGuild> Guilds => _guilds.ToReadOnlyCollection();
        internal IReadOnlyCollection<CachedGlobalUser> Users => _users.ToReadOnlyCollection();

        internal IReadOnlyCollection<ICachedPrivateChannel> PrivateChannels =>
            _dmChannels.Select(x => x.Value as ICachedPrivateChannel).Concat(
                _groupChannels.Select(x => GetChannel(x) as ICachedPrivateChannel))
            .ToReadOnlyCollection(() => _dmChannels.Count + _groupChannels.Count);

        public DataStore(int guildCount, int dmChannelCount)
        {
            double estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
            double estimatedUsersCount = guildCount * AverageUsersPerGuild;
            _channels = new ConcurrentDictionary<ulong, ICachedChannel>(CollectionConcurrencyLevel, (int)(estimatedChannelCount * CollectionMultiplier));
            _dmChannels = new ConcurrentDictionary<ulong, CachedDMChannel>(CollectionConcurrencyLevel, (int)(dmChannelCount * CollectionMultiplier));
            _guilds = new ConcurrentDictionary<ulong, CachedGuild>(CollectionConcurrencyLevel, (int)(guildCount * CollectionMultiplier));
            _users = new ConcurrentDictionary<ulong, CachedGlobalUser>(CollectionConcurrencyLevel, (int)(estimatedUsersCount * CollectionMultiplier));
            _groupChannels = new ConcurrentHashSet<ulong>(CollectionConcurrencyLevel, (int)(10 * CollectionMultiplier));
        }

        internal ICachedChannel GetChannel(ulong id)
        {
            ICachedChannel channel;
            if (_channels.TryGetValue(id, out channel))
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
        internal void AddChannel(ICachedChannel channel)
        {
            _channels[channel.Id] = channel;

            var dmChannel = channel as CachedDMChannel;
            if (dmChannel != null)
                _dmChannels[dmChannel.Recipient.Id] = dmChannel;
            else
            {
                var groupChannel = channel as CachedGroupChannel;
                if (groupChannel != null)
                    _groupChannels.TryAdd(groupChannel.Id);
            }
        }
        internal ICachedChannel RemoveChannel(ulong id)
        {
            ICachedChannel channel;
            if (_channels.TryRemove(id, out channel))
            {
                var dmChannel = channel as CachedDMChannel;
                if (dmChannel != null)
                {
                    CachedDMChannel ignored;
                    _dmChannels.TryRemove(dmChannel.Recipient.Id, out ignored);
                }
                else
                {
                    var groupChannel = channel as CachedGroupChannel;
                    if (groupChannel != null)
                        _groupChannels.TryRemove(id);
                }
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
