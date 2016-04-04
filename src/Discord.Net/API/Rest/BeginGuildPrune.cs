using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BeginGuildPruneRequest : IRestRequest<GetGuildPruneCountResponse>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/prune";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }

        [JsonProperty("days")]
        public int Days { get; set; } = 30;

        public BeginGuildPruneRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
