//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Discord.API
{
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
	internal sealed class KeepAliveCommand : WebSocketMessage<long>
	{
		public KeepAliveCommand() : base(1, EpochTime.GetMilliseconds()) { }
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
			[JsonProperty("large_threshold", NullValueHandling = NullValueHandling.Ignore)]
			public int? LargeThreshold;
			[JsonProperty("compress", NullValueHandling = NullValueHandling.Ignore)]
			public bool? Compress;
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
	internal sealed class ResumedEvent
	{
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval;
	}

	internal sealed class RedirectEvent
	{
		[JsonProperty("url")]
		public string Url;
	}
}
