using System;

namespace Discord
{
	public partial class DiscordClient
	{
		//Debug
		public sealed class LogMessageEventArgs : EventArgs
		{
			public readonly string Message;
			internal LogMessageEventArgs(string msg) { Message = msg; }
		}
		public event EventHandler<LogMessageEventArgs> DebugMessage;
		private void RaiseOnDebugMessage(string message)
		{
			if (DebugMessage != null)
				DebugMessage(this, new LogMessageEventArgs(message));
		}

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

		//Server
		public sealed class ServerEventArgs : EventArgs
		{
			public readonly Server Server;
			internal ServerEventArgs(Server server) { Server = server; }
		}

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

		//Channel
		public sealed class ChannelEventArgs : EventArgs
		{
			public readonly Channel Channel;
			internal ChannelEventArgs(Channel channel) { Channel = channel; }
		}

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

		//User
		public sealed class UserEventArgs : EventArgs
		{
			public readonly User User;
			internal UserEventArgs(User user) { User = user; }
		}
		public event EventHandler<UserEventArgs> UserUpdated;
		private void RaiseUserUpdated(User user)
		{
			if (UserUpdated != null)
				UserUpdated(this, new UserEventArgs(user));
		}

		//Message
		public sealed class MessageEventArgs : EventArgs
		{
			public readonly Message Message;
			internal MessageEventArgs(Message msg) { Message = msg; }
		}

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
		public event EventHandler<MessageEventArgs> MessageAcknowledged;
		private void RaiseMessageAcknowledged(Message msg)
		{
			if (MessageAcknowledged != null)
				MessageAcknowledged(this, new MessageEventArgs(msg));
		}

		//Role
		public sealed class RoleEventArgs : EventArgs
		{
			public readonly Role Role;
			internal RoleEventArgs(Role role) { Role = role; }
		}

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
		public sealed class MemberEventArgs : EventArgs
		{
			public readonly Membership Membership;
			internal MemberEventArgs(Membership membership) { Membership = membership; }
		}

		public event EventHandler<MemberEventArgs> MemberAdded;
		private void RaiseMemberAdded(Membership membership, Server server)
		{
			if (MemberAdded != null)
				MemberAdded(this, new MemberEventArgs(membership));
		}
		public event EventHandler<MemberEventArgs> MemberRemoved;
		private void RaiseMemberRemoved(Membership membership, Server server)
		{
			if (MemberRemoved != null)
				MemberRemoved(this, new MemberEventArgs(membership));
		}
		public event EventHandler<MemberEventArgs> MemberUpdated;
		private void RaiseMemberUpdated(Membership membership, Server server)
		{
			if (MemberUpdated != null)
				MemberUpdated(this, new MemberEventArgs(membership));
		}

		//Status
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

		public event EventHandler<UserEventArgs> PresenceUpdated;
		private void RaisePresenceUpdated(User user)
		{
			if (PresenceUpdated != null)
				PresenceUpdated(this, new UserEventArgs(user));
		}
		public event EventHandler<UserEventArgs> VoiceStateUpdated;
		private void RaiseVoiceStateUpdated(User user)
		{
			if (VoiceStateUpdated != null)
				VoiceStateUpdated(this, new UserEventArgs(user));
		}
		public event EventHandler<UserTypingEventArgs> UserTyping;
		private void RaiseUserTyping(User user, Channel channel)
		{
			if (UserTyping != null)
				UserTyping(this, new UserTypingEventArgs(user, channel));
		}

		//Voice
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

		public event EventHandler<VoiceServerUpdatedEventArgs> VoiceServerUpdated;
		private void RaiseVoiceServerUpdated(Server server, string endpoint)
		{
			if (VoiceServerUpdated != null)
				VoiceServerUpdated(this, new VoiceServerUpdatedEventArgs(server, endpoint));
		}
	}
}
