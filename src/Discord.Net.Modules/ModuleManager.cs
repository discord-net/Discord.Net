using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Modules
{
    public class ModuleManager
	{
		/*public event AsyncEventHandler<ServerEventArgs> ServerEnabled { add { _serverEnabled.Add(value); } remove { _serverEnabled.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverEnabled = new AsyncEvent<ServerEventArgs>(nameof(ServerEnabled));
		public event AsyncEventHandler<ServerEventArgs> ServerDisabled { add { _serverDisabled.Add(value); } remove { _serverDisabled.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverDisabled = new AsyncEvent<ServerEventArgs>(nameof(ServerDisabled));
		public event AsyncEventHandler<ChannelEventArgs> ChannelEnabled { add { _channelEnabled.Add(value); } remove { _channelEnabled.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelEnabled = new AsyncEvent<ChannelEventArgs>(nameof(ChannelEnabled));
		public event AsyncEventHandler<ChannelEventArgs> ChannelDisabled { add { _channelDisabled.Add(value); } remove { _channelDisabled.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelDisabled = new AsyncEvent<ChannelEventArgs>(nameof(ChannelDisabled));*/

		public event AsyncEventHandler<ServerEventArgs> LeftServer { add { _leftServer.Add(value); } remove { _leftServer.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _leftServer = new AsyncEvent<ServerEventArgs>(nameof(LeftServer));
		public event AsyncEventHandler<ServerEventArgs> ServerUpdated { add { _serverUpdated.Add(value); } remove { _serverUpdated.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverUpdated = new AsyncEvent<ServerEventArgs>(nameof(ServerUpdated));
		public event AsyncEventHandler<ServerEventArgs> ServerUnavailable { add { _serverUnavailable.Add(value); } remove { _serverUnavailable.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverUnavailable = new AsyncEvent<ServerEventArgs>(nameof(ServerUnavailable));
		public event AsyncEventHandler<ServerEventArgs> ServerAvailable { add { _serverAvailable.Add(value); } remove { _serverAvailable.Remove(value); } }
		private readonly AsyncEvent<ServerEventArgs> _serverAvailable = new AsyncEvent<ServerEventArgs>(nameof(ServerAvailable));

		public event AsyncEventHandler<BanEventArgs> UserBanned { add { _userBanned.Add(value); } remove { _userBanned.Remove(value); } }
		private readonly AsyncEvent<BanEventArgs> _userBanned = new AsyncEvent<BanEventArgs>(nameof(UserBanned));
		public event AsyncEventHandler<BanEventArgs> UserUnbanned { add { _userUnbanned.Add(value); } remove { _userUnbanned.Remove(value); } }
		private readonly AsyncEvent<BanEventArgs> _userUnbanned = new AsyncEvent<BanEventArgs>(nameof(UserUnbanned));

		public event AsyncEventHandler<ChannelEventArgs> ChannelCreated { add { _channelCreated.Add(value); } remove { _channelCreated.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelCreated = new AsyncEvent<ChannelEventArgs>(nameof(ChannelCreated));
		public event AsyncEventHandler<ChannelEventArgs> ChannelDestroyed { add { _channelDestroyed.Add(value); } remove { _channelDestroyed.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelDestroyed = new AsyncEvent<ChannelEventArgs>(nameof(ChannelDestroyed));
		public event AsyncEventHandler<ChannelEventArgs> ChannelUpdated { add { _channelUpdated.Add(value); } remove { _channelUpdated.Remove(value); } }
		private readonly AsyncEvent<ChannelEventArgs> _channelUpdated = new AsyncEvent<ChannelEventArgs>(nameof(ChannelUpdated));

		public event AsyncEventHandler<RoleEventArgs> RoleCreated { add { _roleCreated.Add(value); } remove { _roleCreated.Remove(value); } }
		private readonly AsyncEvent<RoleEventArgs> _roleCreated = new AsyncEvent<RoleEventArgs>(nameof(RoleCreated));
		public event AsyncEventHandler<RoleEventArgs> RoleUpdated { add { _roleUpdated.Add(value); } remove { _roleUpdated.Remove(value); } }
		private readonly AsyncEvent<RoleEventArgs> _roleUpdated = new AsyncEvent<RoleEventArgs>(nameof(RoleUpdated));
		public event AsyncEventHandler<RoleEventArgs> RoleDeleted { add { _roleDeleted.Add(value); } remove { _roleDeleted.Remove(value); } }
		private readonly AsyncEvent<RoleEventArgs> _roleDeleted = new AsyncEvent<RoleEventArgs>(nameof(RoleDeleted));

		public event AsyncEventHandler<UserEventArgs> UserJoined { add { _userJoined.Add(value); } remove { _userJoined.Remove(value); } }
		private readonly AsyncEvent<UserEventArgs> _userJoined = new AsyncEvent<UserEventArgs>(nameof(UserJoined));
		public event AsyncEventHandler<UserEventArgs> UserLeft { add { _userLeft.Add(value); } remove { _userLeft.Remove(value); } }
		private readonly AsyncEvent<UserEventArgs> _userLeft = new AsyncEvent<UserEventArgs>(nameof(UserLeft));
		public event AsyncEventHandler<UserEventArgs> UserUpdated { add { _userUpdated.Add(value); } remove { _userUpdated.Remove(value); } }
		private readonly AsyncEvent<UserEventArgs> _userUpdated = new AsyncEvent<UserEventArgs>(nameof(UserUpdated));
		public event AsyncEventHandler<UserEventArgs> UserPresenceUpdated { add { _userPresenceUpdated.Add(value); } remove { _userPresenceUpdated.Remove(value); } }
		private readonly AsyncEvent<UserEventArgs> _userPresenceUpdated = new AsyncEvent<UserEventArgs>(nameof(UserPresenceUpdated));
		public event AsyncEventHandler<UserEventArgs> UserVoiceStateUpdated { add { _userVoiceStateUpdated.Add(value); } remove { _userVoiceStateUpdated.Remove(value); } }
		private readonly AsyncEvent<UserEventArgs> _userVoiceStateUpdated = new AsyncEvent<UserEventArgs>(nameof(UserVoiceStateUpdated));
		public event AsyncEventHandler<UserChannelEventArgs> UserIsTypingUpdated { add { _userIsTypingUpdated.Add(value); } remove { _userIsTypingUpdated.Remove(value); } }
		private readonly AsyncEvent<UserChannelEventArgs> _userIsTypingUpdated = new AsyncEvent<UserChannelEventArgs>(nameof(UserIsTypingUpdated));
		public event AsyncEventHandler<UserIsSpeakingEventArgs> UserIsSpeakingUpdated { add { _userIsSpeakingUpdated.Add(value); } remove { _userIsSpeakingUpdated.Remove(value); } }
		private readonly AsyncEvent<UserIsSpeakingEventArgs> _userIsSpeakingUpdated = new AsyncEvent<UserIsSpeakingEventArgs>(nameof(UserIsSpeakingUpdated));

		public event AsyncEventHandler<MessageEventArgs> MessageReceived { add { _messageReceived.Add(value); } remove { _messageReceived.Remove(value); } }
		private readonly AsyncEvent<MessageEventArgs> _messageReceived = new AsyncEvent<MessageEventArgs>(nameof(MessageReceived));
		public event AsyncEventHandler<MessageEventArgs> MessageSent { add { _messageSent.Add(value); } remove { _messageSent.Remove(value); } }
		private readonly AsyncEvent<MessageEventArgs> _messageSent = new AsyncEvent<MessageEventArgs>(nameof(MessageSent));
		public event AsyncEventHandler<MessageEventArgs> MessageDeleted { add { _messageDeleted.Add(value); } remove { _messageDeleted.Remove(value); } }
		private readonly AsyncEvent<MessageEventArgs> _messageDeleted = new AsyncEvent<MessageEventArgs>(nameof(MessageDeleted));
		public event AsyncEventHandler<MessageEventArgs> MessageUpdated { add { _messageUpdated.Add(value); } remove { _messageUpdated.Remove(value); } }
		private readonly AsyncEvent<MessageEventArgs> _messageUpdated = new AsyncEvent<MessageEventArgs>(nameof(MessageUpdated));
		public event AsyncEventHandler<MessageEventArgs> MessageReadRemotely { add { _messageReadRemotely.Add(value); } remove { _messageReadRemotely.Remove(value); } }
		private readonly AsyncEvent<MessageEventArgs> _messageReadRemotely = new AsyncEvent<MessageEventArgs>(nameof(MessageReadRemotely));

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
				client.ChannelCreated += (s, e) =>
				{
					if (_channelCreated.Any && HasServer(e.Server))
						return _channelCreated.Invoke(s, e);
					return TaskHelper.CompletedTask;
				};

				client.UserVoiceStateUpdated += (s, e) =>
				{
					if (_userVoiceStateUpdated.Any && HasServer(e.Server))
						return _userVoiceStateUpdated.Invoke(s, e);
					return TaskHelper.CompletedTask;
				};
				client.UserIsSpeakingUpdated += (s, e) =>
				{
					if (_userIsSpeakingUpdated.Any && HasServer(e.Server))
						return _userIsSpeakingUpdated.Invoke(s, e);
					return TaskHelper.CompletedTask;
				};
			}

			client.ChannelDestroyed += (s, e) =>
			{
				if (_channelDestroyed.Any && HasChannel(e.Channel))
					return _channelDestroyed.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.ChannelUpdated += (s, e) =>
			{
				if (_channelUpdated.Any && HasChannel(e.Channel))
					return _channelUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};

			client.MessageReceived += (s, e) =>
			{
				if (_messageReceived.Any && HasChannel(e.Channel))
					return _messageReceived.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.MessageSent += (s, e) =>
			{
				if (_messageSent.Any && HasChannel(e.Channel))
					return _messageSent.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.MessageDeleted += (s, e) =>
			{
				if (_messageDeleted.Any && HasChannel(e.Channel))
					return _messageDeleted.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.MessageUpdated += (s, e) =>
			{
				if (_messageUpdated.Any && HasChannel(e.Channel))
					return _messageUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.MessageReadRemotely += (s, e) =>
			{
				if (_messageReadRemotely.Any && HasChannel(e.Channel))
					return _messageReadRemotely.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};

			client.RoleCreated += (s, e) =>
			{
				if (_roleCreated.Any && HasIndirectServer(e.Server))
					return _roleCreated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.RoleUpdated += (s, e) =>
			{
				if (_roleUpdated.Any && HasIndirectServer(e.Server))
					return _roleUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.RoleDeleted += (s, e) =>
			{
				if (_roleDeleted.Any && HasIndirectServer(e.Server))
					return _roleDeleted.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};

			client.LeftServer += (s, e) =>
			{
				if (_leftServer.Any && HasIndirectServer(e.Server))
				{
					DisableServer(e.Server);
					return _leftServer.Invoke(s, e);
				}
				return TaskHelper.CompletedTask;
			};
			client.ServerUpdated += (s, e) =>
			{
				if (_serverUpdated.Any && HasIndirectServer(e.Server))
					return _serverUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.ServerUnavailable += (s, e) =>
			{
				if (_serverUnavailable.Any && HasIndirectServer(e.Server))
					return _serverUnavailable.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.ServerAvailable += (s, e) =>
			{
				if (_serverAvailable.Any && HasIndirectServer(e.Server))
					return _serverAvailable.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};

			client.UserJoined += (s, e) =>
			{
				if (_userJoined.Any && HasIndirectServer(e.Server))
					return _userJoined.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.UserLeft += (s, e) =>
			{
				if (_userLeft.Any && HasIndirectServer(e.Server))
					return _userLeft.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.UserUpdated += (s, e) =>
			{
				if (_userUpdated.Any && HasIndirectServer(e.Server))
					return _userUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.UserIsTypingUpdated += (s, e) =>
			{
				if (_userIsTypingUpdated.Any && HasChannel(e.Channel))
					return _userIsTypingUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.UserIsSpeakingUpdated += (s, e) =>
			{
				if (_userIsSpeakingUpdated.Any && HasChannel(e.Channel))
					return _userIsSpeakingUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			//TODO: We aren't getting events from UserPresence if AllowPrivate is enabled, but the server we know that user through isn't on the whitelist
			client.UserPresenceUpdated += (s, e) =>
			{
				if (_userPresenceUpdated.Any && HasIndirectServer(e.Server))
					return _userPresenceUpdated.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.UserBanned += (s, e) =>
			{
				if (_userBanned.Any && HasIndirectServer(e.Server))
					return _userBanned.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
			client.UserUnbanned += (s, e) =>
			{
				if (_userUnbanned.Any && HasIndirectServer(e.Server))
					return _userUnbanned.Invoke(s, e);
				return TaskHelper.CompletedTask;
			};
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
				return _enabledServers.TryAdd(server.Id, server);
        }
		public bool EnableServers(IEnumerable<Server> servers)
		{
			if (servers == null) throw new ArgumentNullException(nameof(servers));
			if (servers.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(servers));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

			bool result = false;
			lock (this)
			{
				foreach (var server in servers)
				{
					if (_enabledServers.TryAdd(server.Id, server))
						result |= true;
                }
            }
			return result;
		}
		public bool EnableChannel(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			lock (this)
				return EnableChannelInternal(channel);
		}
		public bool EnableChannels(IEnumerable<Channel> channels)
		{
			if (channels == null) throw new ArgumentNullException(nameof(channels));
			if (channels.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(channels));
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			bool result = false;
			lock (this)
			{
				foreach (var channel in channels)
				{
					if (EnableChannelInternal(channel))
						result |= true;
				}
			}
			return result;
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
				return true;
			}
			return false;
		}

		public bool DisableServer(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

			lock (this)
				return _enabledServers.TryRemove(server.Id, out server);
		}
		public bool DisableServers(IEnumerable<Server> servers)
		{
			if (servers == null) throw new ArgumentNullException(nameof(servers));
			if (servers.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(servers));
			if (!_useServerWhitelist) throw new InvalidOperationException("This module is not configured to use a server whitelist.");

			bool result = false;
			lock (this)
			{
				foreach (var server in servers)
				{
					Server ignored;
					if (_enabledServers.TryRemove(server.Id, out ignored))
						result |= true;
				}					
			}
			return result;
		}
		public bool DisableChannel(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			lock (this)
				return DisableChannelInternal(channel);
		}
		public bool DisableChannels(IEnumerable<Channel> channels)
		{
			if (channels == null) throw new ArgumentNullException(nameof(channels));
			if (channels.Contains(null)) throw new ArgumentException("Collection cannot contain null.", nameof(channels));
			if (!_useChannelWhitelist) throw new InvalidOperationException("This module is not configured to use a channel whitelist.");

			bool result = false;
			lock (this)
			{
				foreach (var channel in channels)
					result |= DisableChannelInternal(channel);
			}
			return result;
		}
		private bool DisableChannelInternal(Channel channel)
		{
			Channel ignored;
			if (_enabledChannels.TryRemove(channel.Id, out ignored))
			{
				var server = channel.Server;
				if (server != null)
				{
					int value = 0;
					if (_indirectServers.TryGetValue(server.Id, out value))
					{
						value--;
						if (value <= 0)
							_indirectServers.TryRemove(server.Id, out value);
						else
							_indirectServers[server.Id] = value;
					}
				}
				return true;
			}
			return false;
		}
		public bool DisableAll()
		{
			bool result = false;
			if (_useServerWhitelist)
			{
				result |= _enabledServers.Count != 0;
				_enabledServers.Clear();
			}
			if (_useChannelWhitelist)
			{
				result |= _enabledServers.Count != 0;
				_enabledChannels.Clear();
			}
			return result;
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
