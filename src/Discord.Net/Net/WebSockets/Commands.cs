//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Discord.Net.WebSockets
{
	internal static class Commands
	{
		public sealed class KeepAlive : WebSocketMessage<int>
		{
			public KeepAlive() : base(1, GetTimestamp()) { }
			private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			private static int GetTimestamp()
				=> (int)(DateTime.UtcNow - epoch).TotalMilliseconds;
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
		public sealed class JoinVoice : WebSocketMessage<JoinVoice.Data>
		{
			public JoinVoice() : base(4) { }
			public class Data
			{
				[JsonProperty(PropertyName = "guild_id")]
				public string ServerId;
				[JsonProperty(PropertyName = "channel_id")]
				public string ChannelId;
				[JsonProperty(PropertyName = "self_mute")]
				public string SelfMute;
				[JsonProperty(PropertyName = "self_deaf")]
				public string SelfDeaf;
			}
		}
		public sealed class Resume : WebSocketMessage<Resume.Data>
		{
			public Resume() : base(6) { }
			public class Data
			{
				[JsonProperty(PropertyName = "session_id")]
				public string SessionId;
				[JsonProperty(PropertyName = "seq")]
				public int Sequence;
			}
		}
	}
}
