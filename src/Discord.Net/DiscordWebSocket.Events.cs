using Newtonsoft.Json.Linq;
using System;

namespace Discord
{
	internal partial class DiscordWebSocket
	{
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

		public event EventHandler<MessageEventArgs> GotEvent;
		public sealed class MessageEventArgs : EventArgs
		{
			public readonly string Type;
			public readonly JToken Event;
			internal MessageEventArgs(string type, JToken data)
			{
				Type = type;
				Event = data;
			}
		}
		private void RaiseGotEvent(string type, JToken payload)
		{
			if (GotEvent != null)
				GotEvent(this, new MessageEventArgs(type, payload));
		}

		public event EventHandler<DiscordClient.LogMessageEventArgs> OnDebugMessage;
		private void RaiseOnDebugMessage(string message)
		{
			if (OnDebugMessage != null)
				OnDebugMessage(this, new DiscordClient.LogMessageEventArgs(message));
		}
	}
}
