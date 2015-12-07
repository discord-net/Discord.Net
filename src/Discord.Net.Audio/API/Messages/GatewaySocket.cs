//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.Audio.API
{
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
