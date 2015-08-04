using Discord.Models;
using System;

namespace Discord
{
	public partial class DiscordClient
	{
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
		public sealed class MessageCreateEventArgs : EventArgs
		{
			public readonly ChatMessage Message;
			internal MessageCreateEventArgs(ChatMessage msg) { Message = msg; }
		}
		public sealed class MessageEventArgs : EventArgs
		{
			public readonly ChatMessageReference Message;
			internal MessageEventArgs(ChatMessageReference msg) { Message = msg; }
		}
		public sealed class LogMessageEventArgs : EventArgs
		{
			public readonly string Message;
			internal LogMessageEventArgs(string msg) { Message = msg; }
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

		public event EventHandler<LogMessageEventArgs> DebugMessage;
		private void RaiseOnDebugMessage(string message)
		{
			if (DebugMessage != null)
				DebugMessage(this, new LogMessageEventArgs(message));
		}

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

		public event EventHandler LoggedIn;
		private void RaiseLoggedIn()
		{
			if (LoggedIn != null)
				LoggedIn(this, EventArgs.Empty);
		}
		
		public event EventHandler<ServerEventArgs> ServerCreated, ServerDestroyed;
		private void RaiseServerCreated(Server server)
		{
			if (ServerCreated != null)
				ServerCreated(this, new ServerEventArgs(server));
		}
		private void RaiseServerDestroyed(Server server)
		{
			if (ServerDestroyed != null)
				ServerDestroyed(this, new ServerEventArgs(server));
		}

		public event EventHandler<ChannelEventArgs> ChannelCreated, ChannelDestroyed;
		private void RaiseChannelCreated(Channel channel)
		{
			if (ChannelCreated != null)
				ChannelCreated(this, new ChannelEventArgs(channel));
		}
		private void RaiseChannelDestroyed(Channel channel)
		{
			if (ChannelDestroyed != null)
				ChannelDestroyed(this, new ChannelEventArgs(channel));
		}

		public event EventHandler<MessageCreateEventArgs> MessageCreated;
		public event EventHandler<MessageEventArgs> MessageDeleted, MessageUpdated, MessageAcknowledged;
		private void RaiseMessageCreated(ChatMessage msg)
		{
			if (MessageCreated != null)
				MessageCreated(this, new MessageCreateEventArgs(msg));
		}
		private void RaiseMessageDeleted(ChatMessageReference msg)
		{
			if (MessageDeleted != null)
				MessageDeleted(this, new MessageEventArgs(msg));
		}
		private void RaiseMessageUpdated(ChatMessageReference msg)
		{
			if (MessageUpdated != null)
				MessageUpdated(this, new MessageEventArgs(msg));
		}
		private void RaiseMessageAcknowledged(ChatMessageReference msg)
		{
			if (MessageAcknowledged != null)
				MessageAcknowledged(this, new MessageEventArgs(msg));
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
	}
}
