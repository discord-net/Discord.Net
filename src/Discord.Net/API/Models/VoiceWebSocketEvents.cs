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
			public int SSRC;
			[JsonProperty(PropertyName = "port")]
			public int Port;
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
	}
}
