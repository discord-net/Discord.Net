//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API.Models
{
	internal static class VoiceWebSocketCommands
	{
		public sealed class KeepAlive : WebSocketMessage<object>
		{
			public KeepAlive() : base(3, null) { }
		}
		public sealed class Login : WebSocketMessage<Login.Data>
		{
			public Login() : base(0) { }
			public class Data
			{
				[JsonProperty(PropertyName = "server_id")]
				public string ServerId;
				[JsonProperty(PropertyName = "user_id")]
				public string UserId;
				[JsonProperty(PropertyName = "session_id")]
				public string SessionId;
				[JsonProperty(PropertyName = "token")]
				public string Token;
			}
		}
		public sealed class Login2 : WebSocketMessage<Login.Data>
		{
			public Login2() : base(1) { }
			public class Data
			{
				public class PCData
				{
					[JsonProperty(PropertyName = "address")]
					public string Address;
					[JsonProperty(PropertyName = "port")]
					public int Port;
					[JsonProperty(PropertyName = "mode")]
					public string Mode = "xsalsa20_poly1305";
				}
				[JsonProperty(PropertyName = "protocol")]
				public string Protocol = "udp";
				[JsonProperty(PropertyName = "token")]
				public string Token;
			}
		}
	}
}
