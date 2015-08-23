//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API.Models
{
	internal static class TextWebSocketCommands
	{
		public sealed class KeepAlive : WebSocketMessage<int>
		{
			private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			public KeepAlive() : base(1, (int)(DateTime.UtcNow - epoch).TotalMilliseconds) { }
		}
		public sealed class Login : WebSocketMessage<Login.Data>
		{
			public Login() : base(2) { }
			public class Data
			{
				[JsonProperty(PropertyName = "token")]
				public string Token;
				[JsonProperty(PropertyName = "v")]
				public int Version = 2;
				[JsonProperty(PropertyName = "properties")]
				public Dictionary<string, string> Properties = new Dictionary<string, string>();
			}
		}
		public sealed class UpdateStatus : WebSocketMessage<UpdateStatus.Data>
		{
			public UpdateStatus() : base(3) { }
			public class Data
			{
				[JsonProperty(PropertyName = "idle_since")]
				public string IdleSince;
				[JsonProperty(PropertyName = "game_id")]
				public string GameId;
			}
		}
	}
}
