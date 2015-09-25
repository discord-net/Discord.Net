//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.WebSockets.Voice
{
	internal sealed class ReadyEvent
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

	internal sealed class JoinServerEvent
	{
		[JsonProperty("secret_key")]
		public byte[] SecretKey;
		[JsonProperty("mode")]
		public string Mode;
	}

	internal sealed class IsTalkingEvent
	{
		[JsonProperty("user_id")]
		public string UserId;
		[JsonProperty("ssrc")]
		public uint SSRC;
		[JsonProperty("speaking")]
		public bool IsSpeaking;
	}
}
