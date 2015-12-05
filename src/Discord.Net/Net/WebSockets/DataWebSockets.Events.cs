using Newtonsoft.Json.Linq;
using System;

namespace Discord.Net.WebSockets
{
	public sealed class WebSocketEventEventArgs : EventArgs
	{
		public readonly string Type;
		public readonly JToken Payload;
		internal WebSocketEventEventArgs(string type, JToken data)
		{
			Type = type;
			Payload = data;
		}
	}

	public partial class DataWebSocket
	{
		public event EventHandler<WebSocketEventEventArgs> ReceivedEvent;
		private void RaiseReceivedEvent(string type, JToken payload)
		{
			if (ReceivedEvent != null)
				ReceivedEvent(this, new WebSocketEventEventArgs(type, payload));
		}
	}
}
