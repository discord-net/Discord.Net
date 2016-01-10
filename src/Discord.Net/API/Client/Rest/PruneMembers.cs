using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PruneMembersRequest : IRestRequest<PruneMembersResponse>
    {
        string IRestRequest.Method => IsSimulation ? "GET" : "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/prune?days={Days}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; set; }

        public int Days { get; set; } = 30;
        public bool IsSimulation { get; set; } = false;

        public PruneMembersRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }

    public sealed class PruneMembersResponse
    {
        [JsonProperty("pruned")]
        public int Pruned { get; set; }
    }
}
