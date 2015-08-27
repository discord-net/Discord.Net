using System;

namespace Discord
{
	public enum DebugMessageType : byte
	{
		Connection,
		Event,
		Cache,
		WebSocketRawInput, //TODO: Make Http instanced and add a rawoutput event
		WebSocketUnknownInput,
		WebSocketEvent,
		WebSocketUnknownEvent,
		VoiceOutput
	}
	public sealed class LogMessageEventArgs : EventArgs
	{
		public readonly DebugMessageType Type;
		public readonly string Message;
		internal LogMessageEventArgs(DebugMessageType type, string msg) { Type = type; Message = msg; }
	}
	public sealed class ServerEventArgs : EventArgs
	{
		public readonly Server Server;
		internal ServerEventArgs(Server server) { Server = server; }
	}
	public sealed class ChannelEventArgs : EventArgs
	{
		public readonly Channel Channel;
		internal ChannelEventArgs(Channel channel) { Channel = channel; }
	}
	public sealed class UserEventArgs : EventArgs
	{
		public readonly User User;
		internal UserEventArgs(User user) { User = user; }
	}
	public sealed class MessageEventArgs : EventArgs
	{
		public readonly Message Message;
		internal MessageEventArgs(Message msg) { Message = msg; }
	}
	public sealed class RoleEventArgs : EventArgs
	{
		public readonly Role Role;
		internal RoleEventArgs(Role role) { Role = role; }
	}
	public sealed class BanEventArgs : EventArgs
	{
		public readonly User User;
		public readonly Server Server;
		internal BanEventArgs(User user, Server server)
		{
			User = user;
			Server = server;
		}
	}
	public sealed class MemberEventArgs : EventArgs
	{
		public readonly Membership Member;
		internal MemberEventArgs(Membership member) { Member = member; }
	}
	public sealed class UserTypingEventArgs : EventArgs
	{
		public readonly User User;
		public readonly Channel Channel;
		internal UserTypingEventArgs(User user, Channel channel)
		{
			User = user;
			Channel = channel;
		}
	}
	public sealed class VoiceServerUpdatedEventArgs : EventArgs
	{
		public readonly Server Server;
		public readonly string Endpoint;
		internal VoiceServerUpdatedEventArgs(Server server, string endpoint)
		{
			Server = server;
			Endpoint = endpoint;
		}
	}


	public partial class DiscordClient
	{
		//Debug
		public event EventHandler<LogMessageEventArgs> DebugMessage;
		internal void RaiseOnDebugMessage(DebugMessageType type, string message)
		{
			if (DebugMessage != null)
				DebugMessage(this, new LogMessageEventArgs(type, message));
		}

		//General
		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"Connected");
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}
		public event EventHandler Disconnected;
		private void RaiseDisconnected()
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"Disconnected");
			if (Disconnected != null)
				Disconnected(this, EventArgs.Empty);
		}

		//Server
		public event EventHandler<ServerEventArgs> ServerCreated;
		private void RaiseServerCreated(Server server)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"ServerCreated {server.Name} ({server.Id})");
			if (ServerCreated != null)
				ServerCreated(this, new ServerEventArgs(server));
		}
		public event EventHandler<ServerEventArgs> ServerDestroyed;
		private void RaiseServerDestroyed(Server server)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"ServerDestroyed {server.Name} ({server.Id})");
			if (ServerDestroyed != null)
				ServerDestroyed(this, new ServerEventArgs(server));
		}
		public event EventHandler<ServerEventArgs> ServerUpdated;
		private void RaiseServerUpdated(Server server)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"ServerUpdated {server.Name} ({server.Id})");
			if (ServerUpdated != null)
				ServerUpdated(this, new ServerEventArgs(server));
		}

		//User
		public event EventHandler<UserEventArgs> UserUpdated;
		private void RaiseUserUpdated(User user)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"UserUpdated {user.Name} ({user.Id})");
			if (UserUpdated != null)
				UserUpdated(this, new UserEventArgs(user));
		}

		//Channel
		public event EventHandler<ChannelEventArgs> ChannelCreated;
		private void RaiseChannelCreated(Channel channel)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"ChannelCreated {channel.Name} ({channel.Id}) in {channel.Server?.Name} ({channel.ServerId})");
			if (ChannelCreated != null)
				ChannelCreated(this, new ChannelEventArgs(channel));
		}
		public event EventHandler<ChannelEventArgs> ChannelDestroyed;
		private void RaiseChannelDestroyed(Channel channel)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"ChannelDestroyed {channel.Name} ({channel.Id}) in {channel.Server?.Name} ({channel.ServerId})");
			if (ChannelDestroyed != null)
				ChannelDestroyed(this, new ChannelEventArgs(channel));
		}
		public event EventHandler<ChannelEventArgs> ChannelUpdated;
		private void RaiseChannelUpdated(Channel channel)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"ChannelUpdated {channel.Name} ({channel.Id}) in {channel.Server?.Name} ({channel.ServerId})");
			if (ChannelUpdated != null)
				ChannelUpdated(this, new ChannelEventArgs(channel));
		}

		//Message
		public event EventHandler<MessageEventArgs> MessageCreated;
		private void RaiseMessageCreated(Message msg)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MessageCreated {msg.Id} in {msg.Channel?.Name} ({msg.ChannelId})");
			if (MessageCreated != null)
				MessageCreated(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageDeleted;
		private void RaiseMessageDeleted(Message msg)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MessageDeleted {msg.Id} in {msg.Channel?.Name} ({msg.ChannelId})");
			if (MessageDeleted != null)
				MessageDeleted(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageUpdated;
		private void RaiseMessageUpdated(Message msg)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MessageUpdated {msg.Id} in {msg.Channel?.Name} ({msg.ChannelId})");
			if (MessageUpdated != null)
				MessageUpdated(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageRead;
		private void RaiseMessageRead(Message msg)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MessageRead {msg.Id} in {msg.Channel?.Name} ({msg.ChannelId})");
			if (MessageRead != null)
				MessageRead(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageSent;
		private void RaiseMessageSent(Message msg)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MessageSent {msg.Id} in {msg.Channel?.Name} ({msg.ChannelId})");
			if (MessageSent != null)
				MessageSent(this, new MessageEventArgs(msg));
		}

		//Role
		public event EventHandler<RoleEventArgs> RoleCreated;
		private void RaiseRoleCreated(Role role)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"RoleCreated {role.Name} ({role.Id}) in {role.Server?.Name} ({role.ServerId})");
			if (RoleCreated != null)
				RoleCreated(this, new RoleEventArgs(role));
		}
		public event EventHandler<RoleEventArgs> RoleUpdated;
		private void RaiseRoleDeleted(Role role)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"RoleDeleted {role.Name} ({role.Id}) in {role.Server?.Name} ({role.ServerId})");
			if (RoleDeleted != null)
				RoleDeleted(this, new RoleEventArgs(role));
		}
		public event EventHandler<RoleEventArgs> RoleDeleted;
		private void RaiseRoleUpdated(Role role)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"RoleUpdated {role.Name} ({role.Id}) in {role.Server?.Name} ({role.ServerId})");
			if (RoleUpdated != null)
				RoleUpdated(this, new RoleEventArgs(role));
		}

		//Ban
		public event EventHandler<BanEventArgs> BanAdded;
		private void RaiseBanAdded(User user, Server server)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"BanAdded {user.Name} ({user.Id}) in {server.Name} ({server.Id})");
			if (BanAdded != null)
				BanAdded(this, new BanEventArgs(user, server));
		}
		public event EventHandler<BanEventArgs> BanRemoved;
		private void RaiseBanRemoved(User user, Server server)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"BanRemoved {user.Name} ({user.Id}) in {server.Name} ({server.Id})");
			if (BanRemoved != null)
				BanRemoved(this, new BanEventArgs(user, server));
		}

		//Member
		public event EventHandler<MemberEventArgs> MemberAdded;
		private void RaiseMemberAdded(Membership member)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MemberAdded {member.User?.Name} ({member.UserId}) in {member.Server?.Name} ({member.ServerId})");
			if (MemberAdded != null)
				MemberAdded(this, new MemberEventArgs(member));
		}
		public event EventHandler<MemberEventArgs> MemberRemoved;
		private void RaiseMemberRemoved(Membership member)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MemberRemoved {member.User?.Name} ({member.UserId}) in {member.Server?.Name} ({member.ServerId})");
			if (MemberRemoved != null)
				MemberRemoved(this, new MemberEventArgs(member));
		}
		public event EventHandler<MemberEventArgs> MemberUpdated;
		private void RaiseMemberUpdated(Membership member)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"MemberUpdated {member.User?.Name} ({member.UserId}) in {member.Server?.Name} ({member.ServerId})");
			if (MemberUpdated != null)
				MemberUpdated(this, new MemberEventArgs(member));
		}

		//Status
		public event EventHandler<MemberEventArgs> PresenceUpdated;
		private void RaisePresenceUpdated(Membership member)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"PresenceUpdated {member.User?.Name} ({member.UserId}) in {member.Server?.Name} ({member.ServerId})");
			if (PresenceUpdated != null)
				PresenceUpdated(this, new MemberEventArgs(member));
		}
		public event EventHandler<MemberEventArgs> VoiceStateUpdated;
		private void RaiseVoiceStateUpdated(Membership member)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"VoiceStateUpdated {member.User?.Name} ({member.UserId}) in {member.Server?.Name} ({member.ServerId})");
			if (VoiceStateUpdated != null)
				VoiceStateUpdated(this, new MemberEventArgs(member));
		}
		public event EventHandler<UserTypingEventArgs> UserTyping;
		private void RaiseUserTyping(User user, Channel channel)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"VoiceStateUpdated {user.Name} ({user.Id}) in {channel.Name} ({channel.Id})");
			if (UserTyping != null)
				UserTyping(this, new UserTypingEventArgs(user, channel));
		}

		//Voice
		public event EventHandler VoiceConnected;
		private void RaiseVoiceConnected()
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"VoiceConnected");
			if (VoiceConnected != null)
				VoiceConnected(this, EventArgs.Empty);
		}
		public event EventHandler VoiceDisconnected;
		private void RaiseVoiceDisconnected()
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"VoiceDisconnected");
			if (VoiceDisconnected != null)
				VoiceDisconnected(this, EventArgs.Empty);
		}
		public event EventHandler<VoiceServerUpdatedEventArgs> VoiceServerUpdated;
		private void RaiseVoiceServerUpdated(Server server, string endpoint)
		{
			if (_config.EnableDebug)
				RaiseOnDebugMessage(DebugMessageType.Event, $"VoiceServerUpdated {server.Name} ({server.Id})");
			if (VoiceServerUpdated != null)
				VoiceServerUpdated(this, new VoiceServerUpdatedEventArgs(server, endpoint));
		}
	}
}
