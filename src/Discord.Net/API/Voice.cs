//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
	public class GetRegionsResponse : List<GetRegionsResponse.RegionData>
	{
		public sealed class RegionData
		{
			[JsonProperty("sample_hostname")]
			public string Hostname;
			[JsonProperty("sample_port")]
			public int Port;
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("name")]
			public string Name;
		}
	}

	public class GetIceResponse
	{
		[JsonProperty("ttl")]
		public string TTL;
		[JsonProperty("servers")]
		public ServerData[] Servers;

		public sealed class ServerData
		{
			[JsonProperty("url")]
			public string URL;
			[JsonProperty("username")]
			public string Username;
			[JsonProperty("credential")]
			public string Credential;
		}
	}

	//Commands
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

	//Events
	internal sealed class VoiceServerUpdateEvent
	{
		[JsonProperty("guild_id")]
		public string GuildId;
		[JsonProperty("endpoint")]
		public string Endpoint;
		[JsonProperty("token")]
		public string Token;
	}

	//Commands (Voice)
	internal sealed class VoiceLoginCommand : WebSocketMessage<VoiceLoginCommand.Data>
	{
		public VoiceLoginCommand() : base(0) { }
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
	internal sealed class VoiceLogin2Command : WebSocketMessage<VoiceLogin2Command.Data>
	{
		public VoiceLogin2Command() : base(1) { }
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
	internal sealed class VoiceKeepAliveCommand : WebSocketMessage<object>
	{
		public VoiceKeepAliveCommand() : base(3, null) { }
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

	//Events (Voice)
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
		public string UserId;
		[JsonProperty("ssrc")]
		public uint SSRC;
		[JsonProperty("speaking")]
		public bool IsSpeaking;
	}
}
