#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class VoiceServerUpdateEvent
	{
		[JsonProperty("guild_id")]
		public ulong GuildId { get; set; }
        [JsonProperty("endpoint")]
		public string Endpoint { get; set; }
        [JsonProperty("token")]
		public string Token { get; set; }
    }
}
