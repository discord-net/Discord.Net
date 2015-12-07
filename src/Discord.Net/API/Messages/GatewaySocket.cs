//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Discord.API
{
	public enum GatewayOpCodes : byte
	{
		Dispatch = 0,
		Heartbeat = 1,
		Identify = 2,
		StatusUpdate = 3,
		VoiceStateUpdate = 4,
		//VoiceServerPing = 5, (Unused?)
		Resume = 6,
		Redirect = 7,
		RequestGuildMembers = 8
	}

	//Common
	public class WebSocketMessage
	{
		public WebSocketMessage() { }
		public WebSocketMessage(int op) { Operation = op; }

		[JsonProperty("op")]
		public int Operation;
		[JsonProperty("d")]
		public object Payload;
		[JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
		public string Type;
		[JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
		public int? Sequence;
	}
	public abstract class WebSocketMessage<T> : WebSocketMessage
		where T : new()
	{
		public WebSocketMessage() { Payload = new T(); }
		public WebSocketMessage(int op) : base(op) { Payload = new T(); }
		public WebSocketMessage(int op, T payload) : base(op) { Payload = payload; }

		[JsonIgnore]
		public new T Payload
		{
			get
			{
				if (base.Payload is JToken)
					base.Payload = (base.Payload as JToken).ToObject<T>();
				return (T)base.Payload;
			}
			set { base.Payload = value; }
		}
	}

	//Commands
	internal sealed class HeartbeatCommand : WebSocketMessage<long>
	{
		public HeartbeatCommand() : base((int)GatewayOpCodes.Heartbeat, EpochTime.GetMilliseconds()) { }
	}
	internal sealed class IdentifyCommand : WebSocketMessage<IdentifyCommand.Data>
	{
		public IdentifyCommand() : base((int)GatewayOpCodes.Identify) { }
		public class Data
		{
			[JsonProperty("token")]
			public string Token;
			[JsonProperty("v")]
			public int Version = 3;
			[JsonProperty("properties")]
			public Dictionary<string, string> Properties = new Dictionary<string, string>();
			[JsonProperty("large_threshold", NullValueHandling = NullValueHandling.Ignore)]
			public int? LargeThreshold;
			[JsonProperty("compress", NullValueHandling = NullValueHandling.Ignore)]
			public bool? Compress;
        }
	}

	internal sealed class StatusUpdateCommand : WebSocketMessage<StatusUpdateCommand.Data>
	{
		public StatusUpdateCommand() : base((int)GatewayOpCodes.StatusUpdate) { }
		public class Data
		{
			[JsonProperty("idle_since")]
			public long? IdleSince;
			[JsonProperty("game_id")]
			public int? GameId;
		}
	}


	//Commands
	internal sealed class JoinVoiceCommand : WebSocketMessage<JoinVoiceCommand.Data>
	{
		public JoinVoiceCommand() : base((int)GatewayOpCodes.VoiceStateUpdate) { }
		public class Data
		{
			[JsonProperty("guild_id")]
			[JsonConverter(typeof(LongStringConverter))]
			public long ServerId;
			[JsonProperty("channel_id")]
			[JsonConverter(typeof(LongStringConverter))]
			public long ChannelId;
			[JsonProperty("self_mute")]
			public string SelfMute;
			[JsonProperty("self_deaf")]
			public string SelfDeaf;
		}
	}

	//Events
	internal sealed class ResumeCommand : WebSocketMessage<ResumeCommand.Data>
	{
		public ResumeCommand() : base((int)GatewayOpCodes.Resume) { }
		public class Data
		{
			[JsonProperty("session_id")]
			public string SessionId;
			[JsonProperty("seq")]
			public int Sequence;
		}
	}

	//Events
	internal sealed class ReadyEvent
	{
		public sealed class ReadStateInfo
		{
			[JsonProperty("id")]
			public string ChannelId;
			[JsonProperty("mention_count")]
			public int MentionCount;
			[JsonProperty("last_message_id")]
			public string LastMessageId;
		}

		[JsonProperty("v")]
		public int Version;
		[JsonProperty("user")]
		public UserInfo User;
		[JsonProperty("session_id")]
		public string SessionId;
		[JsonProperty("read_state")]
		public ReadStateInfo[] ReadState;
		[JsonProperty("guilds")]
		public ExtendedGuildInfo[] Guilds;
		[JsonProperty("private_channels")]
		public ChannelInfo[] PrivateChannels;
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval;
	}

	internal sealed class RedirectEvent
	{
		[JsonProperty("url")]
		public string Url;
	}
	internal sealed class ResumeEvent
	{
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval;
	}
}
