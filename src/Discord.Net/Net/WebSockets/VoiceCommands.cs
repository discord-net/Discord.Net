//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.Net.WebSockets
{
	internal static class VoiceCommands
	{
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
		public sealed class Login2 : WebSocketMessage<Login2.Data>
		{
			public Login2() : base(1) { }
			public class Data
			{
				public class SocketInfo
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
				[JsonProperty(PropertyName = "data")]
				public SocketInfo SocketData = new SocketInfo();
			}
		}
		public sealed class KeepAlive : WebSocketMessage<object>
		{
			public KeepAlive() : base(3, null) { }
		}
		public sealed class IsTalking : WebSocketMessage<IsTalking.Data>
		{
			public IsTalking() : base(5) { }
			public class Data
			{
				[JsonProperty(PropertyName = "delay")]
				public int Delay;
				[JsonProperty(PropertyName = "speaking")]
				public bool IsSpeaking;
			}
		}
	}
}
