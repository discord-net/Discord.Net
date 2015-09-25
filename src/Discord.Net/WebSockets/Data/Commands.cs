//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.WebSockets.Data
{
	internal sealed class KeepAliveCommand : WebSocketMessage<ulong>
	{
		public KeepAliveCommand() : base(1, GetTimestamp()) { }
		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private static ulong GetTimestamp() => (ulong)(DateTime.UtcNow - epoch).TotalMilliseconds;
	}
	internal sealed class LoginCommand : WebSocketMessage<LoginCommand.Data>
	{
		public LoginCommand() : base(2) { }
		public class Data
		{
			[JsonProperty("token")]
			public string Token;
			[JsonProperty("v")]
			public int Version = 3;
			[JsonProperty("properties")]
			public Dictionary<string, string> Properties = new Dictionary<string, string>();
		}
	}
	internal sealed class UpdateStatusCommand : WebSocketMessage<UpdateStatusCommand.Data>
	{
		public UpdateStatusCommand() : base(3) { }
		public class Data
		{
			[JsonProperty("idle_since")]
			public string IdleSince;
			[JsonProperty("game_id")]
			public string GameId;
		}
	}
	internal sealed class JoinVoiceCommand : WebSocketMessage<JoinVoiceCommand.Data>
	{
		public JoinVoiceCommand() : base(4) { }
		public class Data
		{
			[JsonProperty("guild_id")]
			public string ServerId;
			[JsonProperty("channel_id")]
			public string ChannelId;
			[JsonProperty("self_mute")]
			public string SelfMute;
			[JsonProperty("self_deaf")]
			public string SelfDeaf;
		}
	}
	internal sealed class ResumeCommand : WebSocketMessage<ResumeCommand.Data>
	{
		public ResumeCommand() : base(6) { }
		public class Data
		{
			[JsonProperty("session_id")]
			public string SessionId;
			[JsonProperty("seq")]
			public int Sequence;
		}
	}
}
