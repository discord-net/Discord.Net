#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class VoiceServerUpdateEvent
	{
		[ModelProperty("guild_id")]
		public ulong GuildId { get; set; }
        [ModelProperty("endpoint")]
		public string Endpoint { get; set; }
        [ModelProperty("token")]
		public string Token { get; set; }
    }
}
