using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ListGuildMembersRequest : IRestRequest<GuildMember[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guild/{GuildId}/members";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        [JsonProperty("limit")]
        public int Limit { get; } = 1;
        [JsonProperty("offset")]
        public int Offset { get; } = 0;

        public ListGuildMembersRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
