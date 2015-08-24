//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;

namespace Discord.API.Models
{
	internal static class VoiceWebSocketEvents
	{
		public sealed class Ready
		{
			[JsonProperty(PropertyName = "ssrc")]
			public uint SSRC;
			[JsonProperty(PropertyName = "port")]
			public ushort Port;
			[JsonProperty(PropertyName = "modes")]
			public string[] Modes;
			[JsonProperty(PropertyName = "heartbeat_interval")]
			public int HeartbeatInterval;
		}

		public sealed class JoinServer
		{
			[JsonProperty(PropertyName = "secret_key")]
			public byte[] SecretKey;
			[JsonProperty(PropertyName = "mode")]
			public string Mode;
		}

		public sealed class IsTalking
		{
			[JsonProperty(PropertyName = "user_id")]
			public string UserId;
			[JsonProperty(PropertyName = "ssrc")]
			public uint SSRC;
			[JsonProperty(PropertyName = "speaking")]
			public bool IsSpeaking;
		}
	}
}
