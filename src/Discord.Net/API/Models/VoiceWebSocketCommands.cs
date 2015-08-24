//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord.API.Models
{
	internal static class VoiceWebSocketCommands
	{
		public class WebSocketMessage
		{
			[JsonProperty(PropertyName = "op")]
			public int Operation;
			[JsonProperty(PropertyName = "d")]
			public object Payload;
		}
		internal abstract class WebSocketMessage<T> : WebSocketMessage
			where T : new()
		{
			public WebSocketMessage() { Payload = new T(); }
			public WebSocketMessage(int op) { Operation = op; Payload = new T(); }
			public WebSocketMessage(int op, T payload) { Operation = op; Payload = payload; }

			[JsonIgnore]
			public new T Payload
			{
				get { if (base.Payload is JToken) { base.Payload = (base.Payload as JToken).ToObject<T>(); } return (T)base.Payload; }
				set { base.Payload = value; }
			}
		}

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
