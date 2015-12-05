//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
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

	//Commands
	internal sealed class JoinVoiceCommand : WebSocketMessage<JoinVoiceCommand.Data>
	{
		public JoinVoiceCommand() : base(4) { }
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
	internal sealed class VoiceServerUpdateEvent
	{
		[JsonProperty("guild_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long ServerId;
		[JsonProperty("endpoint")]
		public string Endpoint;
		[JsonProperty("token")]
		public string Token;
	}
}
