using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyGuildEmbedRequest : IRestRequest<Integration>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/embed";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }
        
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }

        public ModifyGuildEmbedRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
