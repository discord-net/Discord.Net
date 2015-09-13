using System;

namespace Discord
{
	public enum LogMessageSeverity : byte
	{
		Error = 1,
		Warning = 2,
		Info = 3,
		Verbose = 4,
		Debug = 5
	}
    public enum LogMessageSource : byte
	{
		Unknown = 0,
		Authentication,
		Cache,
		DataWebSocket,
        MessageQueue,
		Rest,
        VoiceWebSocket,
	}

	public sealed class LogMessageEventArgs : EventArgs
	{
		public readonly LogMessageSeverity Severity;
		public readonly LogMessageSource Source;
		public readonly string Message;
		internal LogMessageEventArgs(LogMessageSeverity severity, LogMessageSource source, string msg) { Severity = severity; Source = source; Message = msg; }
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
		public readonly Member Member;
		internal MemberEventArgs(Member member) { Member = member; }
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
		//General
		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}
		public event EventHandler Disconnected;
		private void RaiseDisconnected()
		{
			if (Disconnected != null)
				Disconnected(this, EventArgs.Empty);
		}
		public event EventHandler<LogMessageEventArgs> LogMessage;
		internal void RaiseOnLog(LogMessageSeverity severity, LogMessageSource source, string message)
		{
			if (LogMessage != null)
				LogMessage(this, new LogMessageEventArgs(severity, source, message));
		}

		//Server
		public event EventHandler<ServerEventArgs> ServerCreated;
		private void RaiseServerCreated(Server server)
		{
			if (ServerCreated != null)
				ServerCreated(this, new ServerEventArgs(server));
		}
		public event EventHandler<ServerEventArgs> ServerDestroyed;
		private void RaiseServerDestroyed(Server server)
		{
			if (ServerDestroyed != null)
				ServerDestroyed(this, new ServerEventArgs(server));
		}
		public event EventHandler<ServerEventArgs> ServerUpdated;
		private void RaiseServerUpdated(Server server)
		{
			if (ServerUpdated != null)
				ServerUpdated(this, new ServerEventArgs(server));
		}

		//User
		public event EventHandler<UserEventArgs> UserUpdated;
		private void RaiseUserUpdated(User user)
		{
			if (UserUpdated != null)
				UserUpdated(this, new UserEventArgs(user));
		}

		//Channel
		public event EventHandler<ChannelEventArgs> ChannelCreated;
		private void RaiseChannelCreated(Channel channel)
		{
			if (ChannelCreated != null)
				ChannelCreated(this, new ChannelEventArgs(channel));
		}
		public event EventHandler<ChannelEventArgs> ChannelDestroyed;
		private void RaiseChannelDestroyed(Channel channel)
		{
			if (ChannelDestroyed != null)
				ChannelDestroyed(this, new ChannelEventArgs(channel));
		}
		public event EventHandler<ChannelEventArgs> ChannelUpdated;
		private void RaiseChannelUpdated(Channel channel)
		{
			if (ChannelUpdated != null)
				ChannelUpdated(this, new ChannelEventArgs(channel));
		}

		//Message
		public event EventHandler<MessageEventArgs> MessageCreated;
		private void RaiseMessageCreated(Message msg)
		{
			if (MessageCreated != null)
				MessageCreated(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageDeleted;
		private void RaiseMessageDeleted(Message msg)
		{
			if (MessageDeleted != null)
				MessageDeleted(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageUpdated;
		private void RaiseMessageUpdated(Message msg)
		{
			if (MessageUpdated != null)
				MessageUpdated(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageRead;
		private void RaiseMessageRead(Message msg)
		{
			if (MessageRead != null)
				MessageRead(this, new MessageEventArgs(msg));
		}
		public event EventHandler<MessageEventArgs> MessageSent;
		private void RaiseMessageSent(Message msg)
		{
			if (MessageSent != null)
				MessageSent(this, new MessageEventArgs(msg));
		}

		//Role
		public event EventHandler<RoleEventArgs> RoleCreated;
		private void RaiseRoleCreated(Role role)
		{
			if (RoleCreated != null)
				RoleCreated(this, new RoleEventArgs(role));
		}
		public event EventHandler<RoleEventArgs> RoleUpdated;
		private void RaiseRoleDeleted(Role role)
		{
			if (RoleDeleted != null)
				RoleDeleted(this, new RoleEventArgs(role));
		}
		public event EventHandler<RoleEventArgs> RoleDeleted;
		private void RaiseRoleUpdated(Role role)
		{
			if (RoleUpdated != null)
				RoleUpdated(this, new RoleEventArgs(role));
		}

		//Ban
		public event EventHandler<BanEventArgs> BanAdded;
		private void RaiseBanAdded(User user, Server server)
		{
			if (BanAdded != null)
				BanAdded(this, new BanEventArgs(user, server));
		}
		public event EventHandler<BanEventArgs> BanRemoved;
		private void RaiseBanRemoved(User user, Server server)
		{
			if (BanRemoved != null)
				BanRemoved(this, new BanEventArgs(user, server));
		}

		//Member
		public event EventHandler<MemberEventArgs> MemberAdded;
		private void RaiseMemberAdded(Member member)
		{
			if (MemberAdded != null)
				MemberAdded(this, new MemberEventArgs(member));
		}
		public event EventHandler<MemberEventArgs> MemberRemoved;
		private void RaiseMemberRemoved(Member member)
		{
			if (MemberRemoved != null)
				MemberRemoved(this, new MemberEventArgs(member));
		}
		public event EventHandler<MemberEventArgs> MemberUpdated;
		private void RaiseMemberUpdated(Member member)
		{
			if (MemberUpdated != null)
				MemberUpdated(this, new MemberEventArgs(member));
		}

		//Status
		public event EventHandler<MemberEventArgs> PresenceUpdated;
		private void RaisePresenceUpdated(Member member)
		{
			if (PresenceUpdated != null)
				PresenceUpdated(this, new MemberEventArgs(member));
		}
		public event EventHandler<MemberEventArgs> VoiceStateUpdated;
		private void RaiseVoiceStateUpdated(Member member)
		{
			if (VoiceStateUpdated != null)
				VoiceStateUpdated(this, new MemberEventArgs(member));
		}
		public event EventHandler<UserTypingEventArgs> UserTyping;
		private void RaiseUserTyping(User user, Channel channel)
		{
			if (UserTyping != null)
				UserTyping(this, new UserTypingEventArgs(user, channel));
		}

		//Voice
		public event EventHandler VoiceConnected;
		private void RaiseVoiceConnected()
		{
			if (VoiceConnected != null)
				VoiceConnected(this, EventArgs.Empty);
		}
		public event EventHandler VoiceDisconnected;
		private void RaiseVoiceDisconnected()
		{
			if (VoiceDisconnected != null)
				VoiceDisconnected(this, EventArgs.Empty);
		}
		public event EventHandler<VoiceServerUpdatedEventArgs> VoiceServerUpdated;
		private void RaiseVoiceServerUpdated(Server server, string endpoint)
		{
			if (VoiceServerUpdated != null)
				VoiceServerUpdated(this, new VoiceServerUpdatedEventArgs(server, endpoint));
		}
	}
}
