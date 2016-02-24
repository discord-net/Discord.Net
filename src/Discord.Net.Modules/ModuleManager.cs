using Discord.Commands;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Modules
{
    public class ModuleManager<T> : ModuleManager
        where T : class, IModule
    {
        public new T Instance => base.Instance as T;

        internal ModuleManager(DiscordClient client, T instance, string name, ModuleFilter filterType)
            : base(client, instance, name, filterType)
        {
        }
    }

    public class ModuleManager
	{
        public event EventHandler<ServerEventArgs> JoinedServer = delegate { };
        public event EventHandler<ServerEventArgs> LeftServer = delegate { };
        public event EventHandler<ServerUpdatedEventArgs> ServerUpdated = delegate { };
        public event EventHandler<ServerEventArgs> ServerUnavailable = delegate { };
        public event EventHandler<ServerEventArgs> ServerAvailable = delegate { };
        
        public event EventHandler<ChannelEventArgs> ChannelCreated = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelDestroyed = delegate { };
        public event EventHandler<ChannelUpdatedEventArgs> ChannelUpdated = delegate { };

        public event EventHandler<RoleEventArgs> RoleCreated = delegate { };
        public event EventHandler<RoleUpdatedEventArgs> RoleUpdated = delegate { };
        public event EventHandler<RoleEventArgs> RoleDeleted = delegate { };

        public event EventHandler<UserEventArgs> UserBanned = delegate { };
        public event EventHandler<UserEventArgs> UserJoined = delegate { };
        public event EventHandler<UserEventArgs> UserLeft = delegate { };
        public event EventHandler<UserUpdatedEventArgs> UserUpdated = delegate { };
        public event EventHandler<UserEventArgs> UserUnbanned = delegate { };
        public event EventHandler<TypingEventArgs> UserIsTyping = delegate { };

        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        public event EventHandler<MessageEventArgs> MessageSent = delegate { };
        public event EventHandler<MessageEventArgs> MessageDeleted = delegate { };
        public event EventHandler<MessageUpdatedEventArgs> MessageUpdated = delegate { };
        public event EventHandler<MessageEventArgs> MessageReadRemotely = delegate { };
        
		private readonly bool _useServerWhitelist, _useChannelWhitelist, _allowAll, _allowPrivate;
		private readonly ConcurrentDictionary<ulong, Server> _enabledServers;
		private readonly ConcurrentDictionary<ulong, IChannel> _enabledChannels;
		private readonly ConcurrentDictionary<ulong, int> _indirectServers;
        private readonly AsyncLock _lock;

        public DiscordClient Client { get; }
        public IModule Instance { get; }
        public string Name { get; }
		public string Id { get; }
		public ModuleFilter FilterType { get; }

        public IEnumerable<Server> EnabledServers => _enabledServers.Select(x => x.Value);
		public IEnumerable<IChannel> EnabledChannels => _enabledChannels.Select(x => x.Value);

		internal ModuleManager(DiscordClient client, IModule instance, string name, ModuleFilter filterType)
		{
            Client = client;
            Instance = instance;
            Name = name;
            FilterType = filterType;

            Id = name.ToLowerInvariant();
            _lock = new AsyncLock();

			_allowAll = filterType == ModuleFilter.None;
			_useServerWhitelist = filterType.HasFlag(ModuleFilter.ServerWhitelist);
			_useChannelWhitelist = filterType.HasFlag(ModuleFilter.ChannelWhitelist);
			_allowPrivate = filterType.HasFlag(ModuleFilter.AlwaysAllowPrivate);

            _enabledServers = new ConcurrentDictionary<ulong, Server>();
			_enabledChannels = new ConcurrentDictionary<ulong, IChannel>();
			_indirectServers = new ConcurrentDictionary<ulong, int>();

			if (_allowAll || _useServerWhitelist) //Server-only events
			{
				client.ChannelCreated += (s, e) => 
                {
                    var server = (e.Channel as PublicChannel)?.Server;
                    if (HasServer(server))
                        ChannelCreated(s, e);
                };
                //TODO: This *is* a channel update if the before/after voice channel is whitelisted
				//client.UserVoiceStateUpdated += (s, e) => { if (HasServer(e.Server)) UserVoiceStateUpdated(s, e); };
			}

			client.ChannelDestroyed += (s, e) => { if (HasChannel(e.Channel)) ChannelDestroyed(s, e); };
			client.ChannelUpdated += (s, e) => { if (HasChannel(e.After)) ChannelUpdated(s, e); };

			client.MessageReceived += (s, e) => { if (HasChannel(e.Channel)) MessageReceived(s, e); };
			client.MessageSent += (s, e) => { if (HasChannel(e.Channel)) MessageSent(s, e); };
			client.MessageDeleted += (s, e) => { if (HasChannel(e.Channel)) MessageDeleted(s, e); };
			client.MessageUpdated += (s, e) => { if (HasChannel(e.Channel)) MessageUpdated(s, e); };
			client.MessageAcknowledged += (s, e) => { if (HasChannel(e.Channel)) MessageReadRemotely(s, e); };

			client.RoleCreated += (s, e) => { if (HasIndirectServer(e.Server)) RoleCreated(s, e); };
			client.RoleUpdated += (s, e) => { if (HasIndirectServer(e.Server)) RoleUpdated(s, e); };
			client.RoleDeleted += (s, e) => { if (HasIndirectServer(e.Server)) RoleDeleted(s, e); };

            client.JoinedServer += (s, e) => { if (_allowAll) JoinedServer(s, e); };
            client.LeftServer += (s, e) => { if (HasIndirectServer(e.Server)) LeftServer(s, e); };
			client.ServerUpdated += (s, e) => { if (HasIndirectServer(e.After)) ServerUpdated(s, e); };
			client.ServerUnavailable += (s, e) => { if (HasIndirectServer(e.Server)) ServerUnavailable(s, e); };
			client.ServerAvailable += (s, e) => { if (HasIndirectServer(e.Server)) ServerAvailable(s, e); };

			client.UserJoined += (s, e) => { if (HasIndirectServer(e.Server)) UserJoined(s, e); };
			client.UserLeft += (s, e) => { if (HasIndirectServer(e.Server)) UserLeft(s, e); };
			//TODO: We aren't getting events from UserPresence if AllowPrivate is enabled, but the server we know that user through isn't on the whitelist
			client.UserUpdated += (s, e) => { if (HasIndirectServer(e.Server)) UserUpdated(s, e); };
			client.UserIsTyping += (s, e) => { if (HasChannel(e.Channel)) UserIsTyping(s, e); };
			client.UserBanned += (s, e) => { if (HasIndirectServer(e.Server)) UserBanned(s, e); };
			client.UserUnbanned += (s, e) => { if (HasIndirectServer(e.Server)) UserUnbanned(s, e); };
		}

		public void CreateCommands(string prefix, Action<CommandGroupBuilder> config)
		{
			var commandService = Client.GetService<CommandService>();
			commandService.CreateGroup(prefix, x =>
			{
				x.Category(Name);
				x.AddCheck(new ModuleChecker(this));
				config(x);
            });

		}
		public bool EnableServer(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

            using (_lock.Lock())
                return EnableServerInternal(server);
		}
		public void EnableServers(IEnumerable<Server> servers)
		{
			if (servers == null) throw new ArgumentNullException(nameof(servers));
			if (servers.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(servers));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

            using (_lock.Lock())
            {
				foreach (var server in servers)
					EnableServerInternal(server);
            }
		}
		private bool EnableServerInternal(Server server) => _enabledServers.TryAdd(server.Id, server);

		public bool DisableServer(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
            if (!_useServerWhitelist) return false;

            using (_lock.Lock())
                return _enabledServers.TryRemove(server.Id, out server);
		}
		public void DisableAllServers()
		{
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");
            if (!_useServerWhitelist) return;

            using (_lock.Lock())					
				_enabledServers.Clear();
		}

		public bool EnableChannel(ITextChannel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

            using (_lock.Lock())
                return EnableChannelInternal(channel);
		}
		public void EnableChannels(IEnumerable<ITextChannel> channels)
		{
			if (channels == null) throw new ArgumentNullException(nameof(channels));
			if (channels.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(channels));
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

            using (_lock.Lock())
            {
				foreach (var channel in channels)
					EnableChannelInternal(channel);
            }
		}
		private bool EnableChannelInternal(ITextChannel channel)
		{
			if (_enabledChannels.TryAdd(channel.Id, channel))
            {
                if (channel.Type != ChannelType.Private)
                {
                    var server = (channel as PublicChannel)?.Server;
                    int value = 0;
                    _indirectServers.TryGetValue(server.Id, out value);
                    value++;
                    _indirectServers[server.Id] = value;
                }
				return true;
			}
			return false;
		}

		public bool DisableChannel(IChannel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (!_useChannelWhitelist) return false;

            IChannel ignored;
            if (_enabledChannels.TryRemove(channel.Id, out ignored))
            {
                using (_lock.Lock())
                {
                    if (channel.Type != ChannelType.Private)
                    {
                        var server = (channel as PublicChannel)?.Server;
                        int value = 0;
                        _indirectServers.TryGetValue(server.Id, out value);
                        value--;
                        if (value <= 0)
                            _indirectServers.TryRemove(server.Id, out value);
                        else
                            _indirectServers[server.Id] = value;
                    }
                    return true;
                }
            }
            return false;
        }
		public void DisableAllChannels()
		{
            if (!_useChannelWhitelist) return;

            using (_lock.Lock())
            {
				_enabledChannels.Clear();
				_indirectServers.Clear();
			}
		}

		public void DisableAll()
		{
			if (_useServerWhitelist)
				DisableAllServers();
			if (_useChannelWhitelist)
				DisableAllChannels();
		}

		internal bool HasServer(Server server) =>
			_allowAll ||
			(_useServerWhitelist && _enabledServers.ContainsKey(server.Id));
		internal bool HasIndirectServer(Server server) =>
			_allowAll ||
			(_useServerWhitelist && _enabledServers.ContainsKey(server.Id)) || 
			(_useChannelWhitelist && _indirectServers.ContainsKey(server.Id));
		internal bool HasChannel(IChannel channel)
		{
			if (_allowAll) return true;
            if (channel.Type == ChannelType.Private) return _allowPrivate;

			if (_useChannelWhitelist && _enabledChannels.ContainsKey(channel.Id)) return true;
			if (_useServerWhitelist && channel.IsPublic)
			{
				var server = (channel as PublicChannel).Server;
				if (server == null) return false;
				if (_enabledServers.ContainsKey(server.Id)) return true;
			}
			return false;
		}
	}
}
