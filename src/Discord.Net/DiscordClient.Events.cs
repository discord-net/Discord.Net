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
		Cache,
		Client,
		DataWebSocket,
        MessageQueue,
		Rest,
        VoiceWebSocket,
	}

	public class DisconnectedEventArgs : EventArgs
	{
		public readonly bool WasUnexpected;
		public readonly Exception Error;

		internal DisconnectedEventArgs(bool wasUnexpected, Exception error) 
		{ 
			WasUnexpected = wasUnexpected; 
			Error = error; 
		}
	}
	public sealed class LogMessageEventArgs : EventArgs
	{
		public LogMessageSeverity Severity { get; }
		public LogMessageSource Source { get; }
		public string Message { get; }

		internal LogMessageEventArgs(LogMessageSeverity severity, LogMessageSource source, string msg) 
		{ 
			Severity = severity; 
			Source = source; 
			Message = msg; 
		}
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
	public sealed class UserIsSpeakingEventArgs : EventArgs
	{
		public Channel Channel => Member.VoiceChannel;
        public string ChannelId => Member.VoiceChannelId;
		public Server Server => Member.Server;
		public string ServerId => Member.ServerId;
		public User User => Member.User;
        public string UserId => Member.UserId;
		public Member Member { get; }
		public bool IsSpeaking { get; }

		internal UserIsSpeakingEventArgs(Member member, bool isSpeaking)
		{
			Member = member;
			IsSpeaking = isSpeaking;
		}
	}
	public sealed class VoicePacketEventArgs
	{
		public string UserId { get; }
		public string ChannelId { get; }
		public byte[] Buffer { get; }
		public int Offset { get; }
		public int Count { get; }

		internal VoicePacketEventArgs(string userId, string channelId, byte[] buffer, int offset, int count)
		{
			UserId = userId;
			Buffer = buffer;
			Offset = offset;
			Count = count;
        }
	}

	public partial class DiscordClient
	{
		//General
		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (Connected != null)
				RaiseEvent(nameof(Connected), () => Connected(this, EventArgs.Empty));
		}
		public event EventHandler<DisconnectedEventArgs> Disconnected;
		private void RaiseDisconnected(DisconnectedEventArgs e)
		{
			if (Disconnected != null)
				RaiseEvent(nameof(Disconnected), () => Disconnected(this, e));
		}
		public event EventHandler<LogMessageEventArgs> LogMessage;
		internal void RaiseOnLog(LogMessageSeverity severity, LogMessageSource source, string message)
		{
			if (LogMessage != null)
				RaiseEvent(nameof(LogMessage), () => LogMessage(this, new LogMessageEventArgs(severity, source, message)));
		}

		//Server
		public event EventHandler<ServerEventArgs> ServerCreated;
		private void RaiseServerCreated(Server server)
		{
			if (ServerCreated != null)
				RaiseEvent(nameof(ServerCreated), () => ServerCreated(this, new ServerEventArgs(server)));
		}
		public event EventHandler<ServerEventArgs> ServerDestroyed;
		private void RaiseServerDestroyed(Server server)
		{
			if (ServerDestroyed != null)
				RaiseEvent(nameof(ServerDestroyed), () => ServerDestroyed(this, new ServerEventArgs(server)));
		}
		public event EventHandler<ServerEventArgs> ServerUpdated;
		private void RaiseServerUpdated(Server server)
		{
			if (ServerUpdated != null)
				RaiseEvent(nameof(ServerUpdated), () => ServerUpdated(this, new ServerEventArgs(server)));
		}

		//Channel
		public event EventHandler<ChannelEventArgs> ChannelCreated;
		private void RaiseChannelCreated(Channel channel)
		{
			if (ChannelCreated != null)
				RaiseEvent(nameof(ChannelCreated), () => ChannelCreated(this, new ChannelEventArgs(channel)));
		}
		public event EventHandler<ChannelEventArgs> ChannelDestroyed;
		private void RaiseChannelDestroyed(Channel channel)
		{
			if (ChannelDestroyed != null)
				RaiseEvent(nameof(ChannelDestroyed), () => ChannelDestroyed(this, new ChannelEventArgs(channel)));
		}
		public event EventHandler<ChannelEventArgs> ChannelUpdated;
		private void RaiseChannelUpdated(Channel channel)
		{
			if (ChannelUpdated != null)
				RaiseEvent(nameof(ChannelUpdated), () => ChannelUpdated(this, new ChannelEventArgs(channel)));
		}

		//Message
		public event EventHandler<MessageEventArgs> MessageCreated;
		private void RaiseMessageCreated(Message msg)
		{
			if (MessageCreated != null)
				RaiseEvent(nameof(MessageCreated), () => MessageCreated(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageDeleted;
		private void RaiseMessageDeleted(Message msg)
		{
			if (MessageDeleted != null)
				RaiseEvent(nameof(MessageDeleted), () => MessageDeleted(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageUpdated;
		private void RaiseMessageUpdated(Message msg)
		{
			if (MessageUpdated != null)
				RaiseEvent(nameof(MessageUpdated), () => MessageUpdated(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageReadRemotely;
		private void RaiseMessageReadRemotely(Message msg)
		{
			if (MessageReadRemotely != null)
				RaiseEvent(nameof(MessageReadRemotely), () => MessageReadRemotely(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageSent;
		private void RaiseMessageSent(Message msg)
		{
			if (MessageSent != null)
				RaiseEvent(nameof(MessageSent), () => MessageSent(this, new MessageEventArgs(msg)));
		}

		//Role
		public event EventHandler<RoleEventArgs> RoleCreated;
		private void RaiseRoleCreated(Role role)
		{
			if (RoleCreated != null)
				RaiseEvent(nameof(RoleCreated), () => RoleCreated(this, new RoleEventArgs(role)));
		}
		public event EventHandler<RoleEventArgs> RoleUpdated;
		private void RaiseRoleDeleted(Role role)
		{
			if (RoleDeleted != null)
				RaiseEvent(nameof(RoleDeleted), () => RoleDeleted(this, new RoleEventArgs(role)));
		}
		public event EventHandler<RoleEventArgs> RoleDeleted;
		private void RaiseRoleUpdated(Role role)
		{
			if (RoleUpdated != null)
				RaiseEvent(nameof(RoleUpdated), () => RoleUpdated(this, new RoleEventArgs(role)));
		}

		//Ban
		public event EventHandler<BanEventArgs> BanAdded;
		private void RaiseBanAdded(string userId, Server server)
		{
			if (BanAdded != null)
				RaiseEvent(nameof(BanAdded), () => BanAdded(this, new BanEventArgs(_users[userId], userId, server)));
		}
		public event EventHandler<BanEventArgs> BanRemoved;
		private void RaiseBanRemoved(string userId, Server server)
		{
			if (BanRemoved != null)
				RaiseEvent(nameof(BanRemoved), () => BanRemoved(this, new BanEventArgs(_users[userId], userId, server)));
		}

		//User
		public event EventHandler<MemberEventArgs> UserAdded;
		private void RaiseUserAdded(Member member)
		{
			if (UserAdded != null)
				RaiseEvent(nameof(UserAdded), () => UserAdded(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserRemoved;
		private void RaiseUserRemoved(Member member)
		{
			if (UserRemoved != null)
				RaiseEvent(nameof(UserRemoved), () => UserRemoved(this, new MemberEventArgs(member)));
		}
		public event EventHandler<UserEventArgs> UserUpdated;
		private void RaiseUserUpdated(User user)
		{
			if (UserUpdated != null)
				RaiseEvent(nameof(UserUpdated), () => UserUpdated(this, new UserEventArgs(user)));
		}
		public event EventHandler<MemberEventArgs> MemberUpdated;
		private void RaiseMemberUpdated(Member member)
		{
			if (MemberUpdated != null)
				RaiseEvent(nameof(MemberUpdated), () => MemberUpdated(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserPresenceUpdated;
		private void RaiseUserPresenceUpdated(Member member)
		{
			if (UserPresenceUpdated != null)
				RaiseEvent(nameof(UserPresenceUpdated), () => UserPresenceUpdated(this, new MemberEventArgs(member)));
		}
		public event EventHandler<MemberEventArgs> UserVoiceStateUpdated;
		private void RaiseUserVoiceStateUpdated(Member member)
		{
			if (UserVoiceStateUpdated != null)
				RaiseEvent(nameof(UserVoiceStateUpdated), () => UserVoiceStateUpdated(this, new MemberEventArgs(member)));
		}
		public event EventHandler<UserTypingEventArgs> UserIsTyping;
		private void RaiseUserIsTyping(User user, Channel channel)
		{
			if (UserIsTyping != null)
				RaiseEvent(nameof(UserIsTyping), () => UserIsTyping(this, new UserTypingEventArgs(user, channel)));
		}
		public event EventHandler<UserIsSpeakingEventArgs> UserIsSpeaking;
		private void RaiseUserIsSpeaking(Member member, bool isSpeaking)
		{
			if (UserIsSpeaking != null)
				RaiseEvent(nameof(UserIsSpeaking), () => UserIsSpeaking(this, new UserIsSpeakingEventArgs(member, isSpeaking)));
		}

		//Voice
		public event EventHandler VoiceConnected;
		private void RaiseVoiceConnected()
		{
			if (VoiceConnected != null)
				RaiseEvent(nameof(UserIsSpeaking), () => VoiceConnected(this, EventArgs.Empty));
		}
		public event EventHandler<DisconnectedEventArgs> VoiceDisconnected;
		private void RaiseVoiceDisconnected(DisconnectedEventArgs e)
		{
			if (VoiceDisconnected != null)
				RaiseEvent(nameof(UserIsSpeaking), () => VoiceDisconnected(this, e));
		}

		public event EventHandler<VoicePacketEventArgs> OnVoicePacket;
		internal void RaiseOnVoicePacket(VoicePacketEventArgs e)
		{
			if (OnVoicePacket != null)
				OnVoicePacket(this, e);
		}
	}
}
