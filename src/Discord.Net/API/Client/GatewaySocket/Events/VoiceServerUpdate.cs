using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
	public class VoiceServerUpdateEvent
	{
		[JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
		public ulong GuildId { get; set; }
        [JsonProperty("endpoint")]
		public string Endpoint { get; set; }
        [JsonProperty("token")]
		public string Token { get; set; }
    }
}
