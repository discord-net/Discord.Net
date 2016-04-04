using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetGuildPruneCountRequest : IRestRequest<GetGuildPruneCountResponse>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/prune";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        [JsonProperty("days")]
        public int Days { get; set; } = 30;

        public GetGuildPruneCountRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }

    public class GetGuildPruneCountResponse
    {
        [JsonProperty("pruned")]
        public int Pruned { get; set; }
    }
}
