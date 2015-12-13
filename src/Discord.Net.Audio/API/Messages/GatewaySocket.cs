//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API
{
	internal sealed class VoiceServerUpdateEvent
	{
		[JsonProperty("guild_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public ulong ServerId;
		[JsonProperty("endpoint")]
		public string Endpoint;
		[JsonProperty("token")]
		public string Token;
	}
}
