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
		Client,
		DataWebSocket,
        MessageQueue,
		Rest,
        VoiceWebSocket,
	}

	public sealed class LogMessageEventArgs : EventArgs
	{
		public LogMessageSeverity Severity { get; }
		public LogMessageSource Source { get; }
		public string Message { get; }

		internal LogMessageEventArgs(LogMessageSeverity severity, LogMessageSource source, string msg) { Severity = severity; Source = source; Message = msg; }
	}
	public sealed class ServerEventArgs : EventArgs
	{
		public Server Server { get; }
		public string ServerId => Server.Id;

		internal ServerEventArgs(Server server) { Server = server; }
	}
	public sealed class ChannelEventArgs : EventArgs
	{
		public Channel Channel { get; }
		public string ChannelId => Channel.Id;
		public Server Server => Channel.Server;
		public string ServerId => Channel.ServerId;

		internal ChannelEventArgs(Channel channel) { Channel = channel; }
	}
	public sealed class UserEventArgs : EventArgs
	{
		public User User { get; }
		public string UserId => User.Id;

		internal UserEventArgs(User user) { User = user; }
	}
	public sealed class MessageEventArgs : EventArgs
	{
		public Message Message { get; }
		public string MessageId => Message.Id;
		public Member Member => Message.Member;
		public Channel Channel => Message.Channel;
		public string ChannelId => Message.ChannelId;
		public Server Server => Message.Server;
		public string ServerId => Message.ServerId;
		public User User => Member.User;
		public string UserId => Message.UserId;

		internal MessageEventArgs(Message msg) { Message = msg; }
	}
	public sealed class RoleEventArgs : EventArgs
	{
		public Role Role { get; }
		public string RoleId => Role.Id;
		public Server Server => Role.Server;
		public string ServerId => Role.ServerId;

		internal RoleEventArgs(Role role) { Role = role; }
	}
	public sealed class BanEventArgs : EventArgs
	{
		public User User { get; }
		public string UserId { get; }
		public Server Server { get; }
		public string ServerId => Server.Id;

		internal BanEventArgs(User user, string userId, Server server)
		{
			User = user;
			UserId = userId;
			Server = server;
		}
	}
	public sealed class MemberEventArgs : EventArgs
	{
		public Member Member { get; }
		public User User => Member.User;
		public string UserId => Member.UserId;
		public Server Server => Member.Server;
		public string ServerId => Member.ServerId;

		internal MemberEventArgs(Member member) { Member = member; }
	}
	public sealed class UserTypingEventArgs : EventArgs
	{
		public Channel Channel { get; }
		public string ChannelId => Channel.Id;
		public Server Server => Channel.Server;
		public string ServerId => Channel.ServerId;
		public User User { get; }
		public string UserId => User.Id;

		internal UserTypingEventArgs(User user, Channel channel)
		{
			User = user;
			Channel = channel;
        }
	}
	/*public sealed class VoiceServerUpdatedEventArgs : EventArgs
	{
		public Server Server { get; }
		public string ServerId => Server.Id;
		public string Endpoint { get; }
		internal VoiceServerUpdatedEventArgs(Server server, string endpoint)
		{
			Server = server;
			Endpoint = endpoint;
		}
	}*/

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
		public event EventHandler<MessageEventArgs> MessageReadRemotely;
		private void RaiseMessageReadRemotely(Message msg)
		{
			if (MessageReadRemotely != null)
				MessageReadRemotely(this, new MessageEventArgs(msg));
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
		private void RaiseBanAdded(string userId, Server server)
		{
			if (BanAdded != null)
				BanAdded(this, new BanEventArgs(_users[userId], userId, server));
		}
		public event EventHandler<BanEventArgs> BanRemoved;
		private void RaiseBanRemoved(string userId, Server server)
		{
			if (BanRemoved != null)
				BanRemoved(this, new BanEventArgs(_users[userId], userId, server));
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
		public event EventHandler<MemberEventArgs> MemberPresenceUpdated;
		private void RaiseMemberPresenceUpdated(Member member)
		{
			if (MemberPresenceUpdated != null)
				MemberPresenceUpdated(this, new MemberEventArgs(member));
		}
		public event EventHandler<MemberEventArgs> MemberVoiceStateUpdated;
		private void RaiseMemberVoiceStateUpdated(Member member)
		{
			if (MemberVoiceStateUpdated != null)
				MemberVoiceStateUpdated(this, new MemberEventArgs(member));
		}
		public event EventHandler<UserTypingEventArgs> UserIsTyping;
		private void RaiseUserIsTyping(User user, Channel channel)
		{
			if (UserIsTyping != null)
				UserIsTyping(this, new UserTypingEventArgs(user, channel));
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
		/*public event EventHandler<VoiceServerUpdatedEventArgs> VoiceServerChanged;
		private void RaiseVoiceServerUpdated(Server server, string endpoint)
		{
			if (VoiceServerChanged != null)
				VoiceServerChanged(this, new VoiceServerUpdatedEventArgs(server, endpoint));
		}*/
	}
}
