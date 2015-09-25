//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.WebSockets.Voice
{
	internal sealed class LoginCommand : WebSocketMessage<LoginCommand.Data>
	{
		public LoginCommand() : base(0) { }
		public class Data
		{
			[JsonProperty("server_id")]
			public string ServerId;
			[JsonProperty("user_id")]
			public string UserId;
			[JsonProperty("session_id")]
			public string SessionId;
			[JsonProperty("token")]
			public string Token;
		}
	}
	internal sealed class Login2Command : WebSocketMessage<Login2Command.Data>
	{
		public Login2Command() : base(1) { }
		public class Data
		{
			public class SocketInfo
			{
				[JsonProperty("address")]
				public string Address;
				[JsonProperty("port")]
				public int Port;
				[JsonProperty("mode")]
				public string Mode = "xsalsa20_poly1305";
			}
			[JsonProperty("protocol")]
			public string Protocol = "udp";
			[JsonProperty("data")]
			public SocketInfo SocketData = new SocketInfo();
		}
	}
	internal sealed class KeepAliveCommand : WebSocketMessage<object>
	{
		public KeepAliveCommand() : base(3, null) { }
	}
	internal sealed class IsTalkingCommand : WebSocketMessage<IsTalkingCommand.Data>
	{
		public IsTalkingCommand() : base(5) { }
		public class Data
		{
			[JsonProperty("delay")]
			public int Delay;
			[JsonProperty("speaking")]
			public bool IsSpeaking;
		}
	}
}
