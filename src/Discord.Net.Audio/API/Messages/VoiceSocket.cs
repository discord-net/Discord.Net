//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API
{
	public enum VoiceOpCodes : byte
	{
		/// <summary> Client --> Server - Used to associate a connection with a token. </summary>
		Identify = 0,
		/// <summary> Client --> Server - Used to specify configuration. </summary>
		SelectProtocol = 1,
		/// <summary> Client <-- Server - Used to notify that the voice connection was successful and informs the client of available protocols. </summary>
		Ready = 2,
		/// <summary> Client <-> Server - Used to keep the connection alive and measure latency. </summary>
		Heartbeat = 3,
		/// <summary> Client <-- Server - Used to provide an encryption key to the client. </summary>
		SessionDescription = 4,
		/// <summary> Client <-> Server - Used to inform that a certain user is speaking. </summary>
		Speaking = 5
	}

	//Commands
	internal sealed class IdentifyCommand : WebSocketMessage<IdentifyCommand.Data>
	{
		public IdentifyCommand() : base((int)VoiceOpCodes.Identify) { }
		public class Data
		{
			[JsonProperty("server_id")]
			[JsonConverter(typeof(LongStringConverter))]
			public ulong ServerId;
			[JsonProperty("user_id")]
			[JsonConverter(typeof(LongStringConverter))]
			public ulong UserId;
			[JsonProperty("session_id")]
			public string SessionId;
			[JsonProperty("token")]
			public string Token;
		}
	}
	internal sealed class SelectProtocolCommand : WebSocketMessage<SelectProtocolCommand.Data>
	{
		public SelectProtocolCommand() : base((int)VoiceOpCodes.SelectProtocol) { }
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
	internal sealed class HeartbeatCommand : WebSocketMessage<long>
	{
		public HeartbeatCommand() : base((int)VoiceOpCodes.Heartbeat, EpochTime.GetMilliseconds()) { }
	}
	internal sealed class SpeakingCommand : WebSocketMessage<SpeakingCommand.Data>
	{
		public SpeakingCommand() : base((int)VoiceOpCodes.Speaking) { }
		public class Data
		{
			[JsonProperty("delay")]
			public int Delay;
			[JsonProperty("speaking")]
			public bool IsSpeaking;
		}
	}

	//Events
	public class VoiceReadyEvent
	{
		[JsonProperty("ssrc")]
		public uint SSRC;
		[JsonProperty("port")]
		public ushort Port;
		[JsonProperty("modes")]
		public string[] Modes;
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval;
	}

	public class JoinServerEvent
	{
		[JsonProperty("secret_key")]
		public byte[] SecretKey;
		[JsonProperty("mode")]
		public string Mode;
	}

	public class IsTalkingEvent
	{
		[JsonProperty("user_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public ulong UserId;
		[JsonProperty("ssrc")]
		public uint SSRC;
		[JsonProperty("speaking")]
		public bool IsSpeaking;
	}
}
