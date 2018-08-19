using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    internal class ClientState
    {
        private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
        private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
        private const double CollectionMultiplier = 1.05; //Add 5% buffer to handle growth

        private readonly ConcurrentDictionary<ulong, SocketChannel> _channels;
        private readonly ConcurrentDictionary<ulong, SocketDMChannel> _dmChannels;
        private readonly ConcurrentHashSet<ulong> _groupChannels;
        private readonly ConcurrentDictionary<ulong, SocketGuild> _guilds;
        private readonly ConcurrentDictionary<ulong, SocketGlobalUser> _users;

        public ClientState(int guildCount, int dmChannelCount)
        {
            var estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
            var estimatedUsersCount = guildCount * AverageUsersPerGuild;
            _channels = new ConcurrentDictionary<ulong, SocketChannel>(ConcurrentHashSet.DefaultConcurrencyLevel,
                (int)(estimatedChannelCount * CollectionMultiplier));
            _dmChannels = new ConcurrentDictionary<ulong, SocketDMChannel>(ConcurrentHashSet.DefaultConcurrencyLevel,
                (int)(dmChannelCount * CollectionMultiplier));
            _guilds = new ConcurrentDictionary<ulong, SocketGuild>(ConcurrentHashSet.DefaultConcurrencyLevel,
                (int)(guildCount * CollectionMultiplier));
            _users = new ConcurrentDictionary<ulong, SocketGlobalUser>(ConcurrentHashSet.DefaultConcurrencyLevel,
                (int)(estimatedUsersCount * CollectionMultiplier));
            _groupChannels = new ConcurrentHashSet<ulong>(ConcurrentHashSet.DefaultConcurrencyLevel,
                (int)(10 * CollectionMultiplier));
        }

        internal IReadOnlyCollection<SocketChannel> Channels => _channels.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();

        internal IReadOnlyCollection<SocketGroupChannel> GroupChannels => _groupChannels
            .Select(x => GetChannel(x) as SocketGroupChannel).ToReadOnlyCollection(_groupChannels);

        internal IReadOnlyCollection<SocketGuild> Guilds => _guilds.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketGlobalUser> Users => _users.ToReadOnlyCollection();

        internal IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels =>
            _dmChannels.Select(x => x.Value as ISocketPrivateChannel).Concat(
                    _groupChannels.Select(x => GetChannel(x) as ISocketPrivateChannel))
                .ToReadOnlyCollection(() => _dmChannels.Count + _groupChannels.Count);

        internal SocketChannel GetChannel(ulong id)
        {
            return _channels.TryGetValue(id, out var channel) ? channel : null;
        }

        internal SocketDMChannel GetDMChannel(ulong userId)
        {
            return _dmChannels.TryGetValue(userId, out var channel) ? channel : null;
        }

        internal void AddChannel(SocketChannel channel)
        {
            _channels[channel.Id] = channel;

            switch (channel)
            {
                case SocketDMChannel dmChannel:
                    _dmChannels[dmChannel.Recipient.Id] = dmChannel;
                    break;
                case SocketGroupChannel groupChannel:
                    _groupChannels.TryAdd(groupChannel.Id);
                    break;
            }
        }

        internal SocketChannel RemoveChannel(ulong id)
        {
            if (!_channels.TryRemove(id, out var channel)) return null;
            switch (channel)
            {
                case SocketDMChannel dmChannel:
                    _dmChannels.TryRemove(dmChannel.Recipient.Id, out var ignored);
                    break;
                case SocketGroupChannel groupChannel:
                    _groupChannels.TryRemove(id);
                    break;
            }

            return channel;

        }

        internal SocketGuild GetGuild(ulong id)
        {
            return _guilds.TryGetValue(id, out var guild) ? guild : null;
        }

        internal void AddGuild(SocketGuild guild) => _guilds[guild.Id] = guild;

        internal SocketGuild RemoveGuild(ulong id)
        {
            return _guilds.TryRemove(id, out var guild) ? guild : null;
        }

        internal SocketGlobalUser GetUser(ulong id)
        {
            return _users.TryGetValue(id, out var user) ? user : null;
        }

        internal SocketGlobalUser GetOrAddUser(ulong id, Func<ulong, SocketGlobalUser> userFactory) =>
            _users.GetOrAdd(id, userFactory);

        internal SocketGlobalUser RemoveUser(ulong id)
        {
            return _users.TryRemove(id, out var user) ? user : null;
        }
    }
}
