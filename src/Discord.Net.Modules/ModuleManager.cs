using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Modules
{
    public class ModuleManager
	{
		public event EventHandler<ServerEventArgs> ServerEnabled;
		public event EventHandler<ServerEventArgs> ServerDisabled;
		public event EventHandler<ChannelEventArgs> ChannelEnabled;
		public event EventHandler<ChannelEventArgs> ChannelDisabled;

		public event EventHandler<ServerEventArgs> LeftServer;
		public event EventHandler<ServerEventArgs> ServerUpdated;
		public event EventHandler<ServerEventArgs> ServerUnavailable;
		public event EventHandler<ServerEventArgs> ServerAvailable;

		public event EventHandler<BanEventArgs> UserBanned;
		public event EventHandler<BanEventArgs> UserUnbanned;

		public event EventHandler<ChannelEventArgs> ChannelCreated;
		public event EventHandler<ChannelEventArgs> ChannelDestroyed;
		public event EventHandler<ChannelEventArgs> ChannelUpdated;

		public event EventHandler<RoleEventArgs> RoleCreated;
		public event EventHandler<RoleEventArgs> RoleUpdated;
		public event EventHandler<RoleEventArgs> RoleDeleted;

		public event EventHandler<UserEventArgs> UserJoined;
		public event EventHandler<UserEventArgs> UserLeft;
		public event EventHandler<UserEventArgs> UserUpdated;
		public event EventHandler<UserEventArgs> UserPresenceUpdated;
		public event EventHandler<UserEventArgs> UserVoiceStateUpdated;
		public event EventHandler<UserChannelEventArgs> UserIsTypingUpdated;

		public event EventHandler<MessageEventArgs> MessageReceived;
		public event EventHandler<MessageEventArgs> MessageSent;
		public event EventHandler<MessageEventArgs> MessageDeleted;
		public event EventHandler<MessageEventArgs> MessageUpdated;
		public event EventHandler<MessageEventArgs> MessageReadRemotely;

		private readonly DiscordClient _client;
		private readonly string _name, _id;
		private readonly FilterType _filterType;
		private readonly bool _useServerWhitelist, _useChannelWhitelist, _allowAll, _allowPrivate;
		private readonly ConcurrentDictionary<long, Server> _enabledServers;
		private readonly ConcurrentDictionary<long, Channel> _enabledChannels;
		private readonly ConcurrentDictionary<long, int> _indirectServers;

		public DiscordClient Client => _client;
		public string Name => _name;
		public string Id => _id;
		public FilterType FilterType => _filterType;
		public IEnumerable<Server> EnabledServers => _enabledServers.Select(x => x.Value);
		public IEnumerable<Channel> EnabledChannels => _enabledChannels.Select(x => x.Value);

		internal ModuleManager(DiscordClient client, string name, FilterType filterType)
		{
			_client = client;
			_name = name;
			_id = name.ToLowerInvariant();

			_filterType = filterType;
			_allowAll = filterType == FilterType.Unrestricted;
			_useServerWhitelist = filterType.HasFlag(FilterType.ServerWhitelist);
			_useChannelWhitelist = filterType.HasFlag(FilterType.ChannelWhitelist);
			_allowPrivate = filterType.HasFlag(FilterType.AllowPrivate);

			_enabledServers = new ConcurrentDictionary<long, Server>();
			_enabledChannels = new ConcurrentDictionary<long, Channel>();
			_indirectServers = new ConcurrentDictionary<long, int>();

			if (_allowAll || _useServerWhitelist) //Server-only events
			{
				client.ChannelCreated += (s, e) => { if (ChannelCreated != null && HasServer(e.Server)) ChannelCreated(s, e); };
				client.UserVoiceStateUpdated += (s, e) => { if (UserVoiceStateUpdated != null && HasServer(e.Server)) UserVoiceStateUpdated(s, e); };
			}

			client.ChannelDestroyed += (s, e) => { if (ChannelDestroyed != null && HasChannel(e.Channel)) ChannelDestroyed(s, e); };
			client.ChannelUpdated += (s, e) => { if (ChannelUpdated != null && HasChannel(e.Channel)) ChannelUpdated(s, e); };

			client.MessageReceived += (s, e) => { if (MessageReceived != null && HasChannel(e.Channel)) MessageReceived(s, e); };
			client.MessageSent += (s, e) => { if (MessageSent != null && HasChannel(e.Channel)) MessageSent(s, e); };
			client.MessageDeleted += (s, e) => { if (MessageDeleted != null && HasChannel(e.Channel)) MessageDeleted(s, e); };
			client.MessageUpdated += (s, e) => { if (MessageUpdated != null && HasChannel(e.Channel)) MessageUpdated(s, e); };
			client.MessageAcknowledged += (s, e) => { if (MessageReadRemotely != null && HasChannel(e.Channel)) MessageReadRemotely(s, e); };

			client.RoleCreated += (s, e) => { if (RoleCreated != null && HasIndirectServer(e.Server)) RoleCreated(s, e); };
			client.RoleUpdated += (s, e) => { if (RoleUpdated != null && HasIndirectServer(e.Server)) RoleUpdated(s, e); };
			client.RoleDeleted += (s, e) => { if (RoleDeleted != null && HasIndirectServer(e.Server)) RoleDeleted(s, e); };

			client.LeftServer += (s, e) => { if (LeftServer != null && HasIndirectServer(e.Server)) { DisableServer(e.Server); LeftServer(s, e); } };
			client.ServerUpdated += (s, e) => { if (ServerUpdated != null && HasIndirectServer(e.Server)) ServerUpdated(s, e); };
			client.ServerUnavailable += (s, e) => { if (ServerUnavailable != null && HasIndirectServer(e.Server)) ServerUnavailable(s, e); };
			client.ServerAvailable += (s, e) => { if (ServerAvailable != null && HasIndirectServer(e.Server)) ServerAvailable(s, e); };

			client.UserJoined += (s, e) => { if (UserJoined != null && HasIndirectServer(e.Server)) UserJoined(s, e); };
			client.UserLeft += (s, e) => { if (UserLeft != null && HasIndirectServer(e.Server)) UserLeft(s, e); };
			client.UserUpdated += (s, e) => { if (UserUpdated != null && HasIndirectServer(e.Server)) UserUpdated(s, e); };
			client.UserIsTypingUpdated += (s, e) => { if (UserIsTypingUpdated != null && HasChannel(e.Channel)) UserIsTypingUpdated(s, e); };
			//TODO: We aren't getting events from UserPresence if AllowPrivate is enabled, but the server we know that user through isn't on the whitelist
			client.UserPresenceUpdated += (s, e) => { if (UserPresenceUpdated != null && HasIndirectServer(e.Server)) UserPresenceUpdated(s, e); };
			client.UserBanned += (s, e) => { if (UserBanned != null && HasIndirectServer(e.Server)) UserBanned(s, e); };
			client.UserUnbanned += (s, e) => { if (UserUnbanned != null && HasIndirectServer(e.Server)) UserUnbanned(s, e); };
		}

		public void CreateCommands(string prefix, Action<CommandGroupBuilder> config)
		{
			var commandService = _client.Commands(true);
			commandService.CreateGroup(prefix, x =>
			{
				x.Category(_name);
				x.AddCheck(new ModuleChecker(this));
				config(x);
            });

		}
		public bool EnableServer(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

			lock (this)
				return EnableServerInternal(server);
		}
		public void EnableServers(IEnumerable<Server> servers)
		{
			if (servers == null) throw new ArgumentNullException(nameof(servers));
			if (servers.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(servers));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

			lock (this)
			{
				foreach (var server in servers)
					EnableServerInternal(server);
            }
		}
		private bool EnableServerInternal(Server server)
		{
			if (_enabledServers.TryAdd(server.Id, server))
			{
				if (ServerEnabled != null)
					ServerEnabled(this, new ServerEventArgs(server));
				return true;
			}
			return false;
		}

		public bool DisableServer(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

			lock (this)
			{
				if (_enabledServers.TryRemove(server.Id, out server))
				{
					if (ServerDisabled != null)
						ServerDisabled(this, new ServerEventArgs(server));
					return true;
				}
				return false;
			}
		}
		public void DisableAllServers()
		{
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

			lock (this)
			{
				if (ServerDisabled != null)
				{
					foreach (var server in _enabledServers)
						ServerDisabled(this, new ServerEventArgs(server.Value));
				}
					
				_enabledServers.Clear();
			}
		}

		public bool EnableChannel(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			lock (this)
				return EnableChannelInternal(channel);
		}
		public void EnableChannels(IEnumerable<Channel> channels)
		{
			if (channels == null) throw new ArgumentNullException(nameof(channels));
			if (channels.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(channels));
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			lock (this)
			{
				foreach (var channel in channels)
					EnableChannelInternal(channel);
            }
		}
		private bool EnableChannelInternal(Channel channel)
		{
			if (_enabledChannels.TryAdd(channel.Id, channel))
			{
				var server = channel.Server;
				if (server != null)
				{
					int value = 0;
					_indirectServers.TryGetValue(server.Id, out value);
					value++;
					_indirectServers[server.Id] = value;
				}
				if (ChannelEnabled != null)
					ChannelEnabled(this, new ChannelEventArgs(channel));
				return true;
			}
			return false;
		}

		public bool DisableChannel(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			lock (this)
			{
				Channel ignored;
				if (_enabledChannels.TryRemove(channel.Id, out ignored))
				{
					var server = channel.Server;
					if (server != null)
					{
						int value = 0;
						_indirectServers.TryGetValue(server.Id, out value);
						value--;
						if (value <= 0)
							_indirectServers.TryRemove(server.Id, out value);
						else
							_indirectServers[server.Id] = value;
					}
					if (ChannelDisabled != null)
						ChannelDisabled(this, new ChannelEventArgs(channel));
					return true;
				}
				return false;
			}
		}
		public void DisableAllChannels()
		{
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			lock (this)
			{
				if (ChannelDisabled != null)
				{
					foreach (var channel in _enabledChannels)
						ChannelDisabled(this, new ChannelEventArgs(channel.Value));
				}

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
			_useServerWhitelist && _enabledServers.ContainsKey(server.Id);
		internal bool HasIndirectServer(Server server) =>
			_allowAll ||
			(_useServerWhitelist && _enabledServers.ContainsKey(server.Id)) || 
			(_useChannelWhitelist && _indirectServers.ContainsKey(server.Id));
		internal bool HasChannel(Channel channel)
		{
			if (_allowAll) return true;
            if (channel.IsPrivate) return _allowPrivate;

			if (_useChannelWhitelist && _enabledChannels.ContainsKey(channel.Id)) return true;
			if (_useServerWhitelist)
			{
				var server = channel.Server;
				if (server == null) return false;
				if (_enabledServers.ContainsKey(server.Id)) return true;
			}
			return false;
		}
	}
}
