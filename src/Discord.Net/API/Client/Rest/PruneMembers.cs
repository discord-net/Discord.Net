using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PruneMembersRequest : IRestRequest<PruneMembersResponse>
    {
        string IRestRequest.Method => IsSimulation ? "GET" : "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/prune?days={Days}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; set; }

        public int Days { get; set; } = 30;
        public bool IsSimulation { get; set; } = false;

        public PruneMembersRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }

    public class PruneMembersResponse
    {
        [JsonProperty("pruned")]
        public int Pruned { get; set; }
    }
}
