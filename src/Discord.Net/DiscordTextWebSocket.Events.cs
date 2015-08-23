using Newtonsoft.Json.Linq;
using System;

namespace Discord
{
	internal partial class DiscordTextWebSocket
	{
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
	}
}
