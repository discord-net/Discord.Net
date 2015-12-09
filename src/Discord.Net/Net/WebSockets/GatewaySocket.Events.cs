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

	public partial class GatewaySocket
	{
		public event EventHandler<WebSocketEventEventArgs> ReceivedDispatch;
		private void RaiseReceivedDispatch(string type, JToken payload)
		{
			if (ReceivedDispatch != null)
				ReceivedDispatch(this, new WebSocketEventEventArgs(type, payload));
		}
	}
}
