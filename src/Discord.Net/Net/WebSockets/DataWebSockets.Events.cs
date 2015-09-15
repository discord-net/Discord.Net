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

	internal partial class DataWebSocket
	{
		public event EventHandler<WebSocketEventEventArgs> ReceievedEvent;
		private void RaiseReceievedEvent(string type, JToken payload)
		{
			if (ReceievedEvent != null)
				ReceievedEvent(this, new WebSocketEventEventArgs(type, payload));
		}
	}
}
