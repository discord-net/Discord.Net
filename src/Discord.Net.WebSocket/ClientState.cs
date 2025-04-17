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
        private readonly ConcurrentDictionary<ulong, SocketGuild> _guilds;
        private readonly ConcurrentDictionary<ulong, SocketGlobalUser> _users;
        private readonly ConcurrentHashSet<ulong> _groupChannels;
        private readonly ConcurrentDictionary<ulong, SocketApplicationCommand> _commands;
        private readonly ConcurrentDictionary<ulong, SocketEntitlement> _entitlements;
        private readonly ConcurrentDictionary<ulong, SocketSubscription> _subscriptions;

        internal IReadOnlyCollection<SocketChannel> Channels => _channels.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketGroupChannel> GroupChannels => _groupChannels.Select(x => GetChannel(x) as SocketGroupChannel).ToReadOnlyCollection(_groupChannels);
        internal IReadOnlyCollection<SocketGuild> Guilds => _guilds.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketGlobalUser> Users => _users.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketApplicationCommand> Commands => _commands.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketEntitlement> Entitlements => _entitlements.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketSubscription> Subscriptions => _subscriptions.ToReadOnlyCollection();

        internal IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels =>
            _dmChannels.Select(x => x.Value as ISocketPrivateChannel).Concat(
                _groupChannels.Select(x => GetChannel(x) as ISocketPrivateChannel))
            .ToReadOnlyCollection(() => _dmChannels.Count + _groupChannels.Count);

        public ClientState(int guildCount, int dmChannelCount)
        {
            double estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
            double estimatedUsersCount = guildCount * AverageUsersPerGuild;
            _channels = new ConcurrentDictionary<ulong, SocketChannel>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(estimatedChannelCount * CollectionMultiplier));
            _dmChannels = new ConcurrentDictionary<ulong, SocketDMChannel>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(dmChannelCount * CollectionMultiplier));
            _guilds = new ConcurrentDictionary<ulong, SocketGuild>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(guildCount * CollectionMultiplier));
            _users = new ConcurrentDictionary<ulong, SocketGlobalUser>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(estimatedUsersCount * CollectionMultiplier));
            _groupChannels = new ConcurrentHashSet<ulong>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(10 * CollectionMultiplier));
            _commands = new ConcurrentDictionary<ulong, SocketApplicationCommand>();
            _entitlements = new();
            _subscriptions = new();
        }

        internal SocketChannel GetChannel(ulong id)
        {
            if (_channels.TryGetValue(id, out SocketChannel channel))
                return channel;
            return null;
        }
        internal SocketDMChannel GetDMChannel(ulong userId)
        {
            if (_dmChannels.TryGetValue(userId, out SocketDMChannel channel))
                return channel;
            return null;
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
            if (_channels.TryRemove(id, out SocketChannel channel))
            {
                switch (channel)
                {
                    case SocketDMChannel dmChannel:
                        _dmChannels.TryRemove(dmChannel.Recipient.Id, out _);
                        break;
                    case SocketGroupChannel _:
                        _groupChannels.TryRemove(id);
                        break;
                }
                return channel;
            }
            return null;
        }
        internal void PurgeAllChannels()
        {
            foreach (var guild in _guilds.Values)
                guild.PurgeChannelCache(this);

            PurgeDMChannels();
        }
        internal void PurgeDMChannels()
        {
            foreach (var channel in _dmChannels.Values)
                _channels.TryRemove(channel.Id, out _);

            _dmChannels.Clear();
        }

        internal SocketGuild GetGuild(ulong id)
        {
            if (_guilds.TryGetValue(id, out SocketGuild guild))
                return guild;
            return null;
        }
        internal void AddGuild(SocketGuild guild)
        {
            _guilds[guild.Id] = guild;
        }
        internal SocketGuild RemoveGuild(ulong id)
        {
            if (_guilds.TryRemove(id, out SocketGuild guild))
            {
                guild.PurgeChannelCache(this);
                guild.PurgeUserCache();
                return guild;
            }
            return null;
        }

        internal SocketGlobalUser GetUser(ulong id)
        {
            if (_users.TryGetValue(id, out SocketGlobalUser user))
                return user;
            return null;
        }
        internal SocketGlobalUser GetOrAddUser(ulong id, Func<ulong, SocketGlobalUser> userFactory)
        {
            return _users.GetOrAdd(id, userFactory);
        }
        internal SocketGlobalUser RemoveUser(ulong id)
        {
            if (_users.TryRemove(id, out SocketGlobalUser user))
                return user;
            return null;
        }
        internal void PurgeUsers()
        {
            foreach (var guild in _guilds.Values)
                guild.PurgeUserCache();
        }

        internal SocketApplicationCommand GetCommand(ulong id)
        {
            if (_commands.TryGetValue(id, out SocketApplicationCommand command))
                return command;
            return null;
        }
        internal void AddCommand(SocketApplicationCommand command)
        {
            _commands[command.Id] = command;
        }
        internal SocketApplicationCommand GetOrAddCommand(ulong id, Func<ulong, SocketApplicationCommand> commandFactory)
        {
            return _commands.GetOrAdd(id, commandFactory);
        }
        internal SocketApplicationCommand RemoveCommand(ulong id)
        {
            if (_commands.TryRemove(id, out SocketApplicationCommand command))
                return command;
            return null;
        }
        internal void PurgeCommands(Func<SocketApplicationCommand, bool> precondition)
        {
            var ids = _commands.Where(x => precondition(x.Value)).Select(x => x.Key);

            foreach (var id in ids)
                _commands.TryRemove(id, out var _);
        }

        internal void AddEntitlement(ulong id, SocketEntitlement entitlement)
        {
            _entitlements.TryAdd(id, entitlement);
        }

        internal SocketEntitlement GetEntitlement(ulong id)
        {
            if (_entitlements.TryGetValue(id, out var entitlement))
                return entitlement;
            return null;
        }

        internal SocketEntitlement GetOrAddEntitlement(ulong id, Func<ulong, SocketEntitlement> entitlementFactory)
        {
            return _entitlements.GetOrAdd(id, entitlementFactory);
        }

        internal SocketEntitlement RemoveEntitlement(ulong id)
        {
            if(_entitlements.TryRemove(id, out var entitlement))
                return entitlement;
            return null;
        }

        internal void AddSubscription(ulong id, SocketSubscription subscription)
        {
            _subscriptions.TryAdd(id, subscription);
        }

        internal SocketSubscription GetSubscription(ulong id)
        {
            if (_subscriptions.TryGetValue(id, out var subscription))
                return subscription;
            return null;
        }

        internal SocketSubscription GetOrAddSubscription(ulong id, Func<ulong, SocketSubscription> subscriptionFactory)
        {
            return _subscriptions.GetOrAdd(id, subscriptionFactory);
        }

        internal SocketSubscription RemoveSubscription(ulong id)
        {
            if (_subscriptions.TryRemove(id, out var subscription))
                return subscription;
            return null;
        }
    }
}
