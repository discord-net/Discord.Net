using Newtonsoft.Json.Linq;
using System;

namespace Discord.WebSockets.Data
{
	internal sealed class WebSocketEventEventArgs : EventArgs
	{
		public readonly string Type;
		public readonly JToken Payload;
		internal WebSocketEventEventArgs(string type, JToken data)
		{
			Type = type;
			Payload = data;
		}
	}

	internal partial class DataWebSocket
	{
		internal event EventHandler<WebSocketEventEventArgs> ReceivedEvent;
		private void RaiseReceivedEvent(string type, JToken payload)
		{
			if (ReceivedEvent != null)
				ReceivedEvent(this, new WebSocketEventEventArgs(type, payload));
		}
	}
}
