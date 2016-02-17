using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetBansRequest : IRestRequest<UserReference[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/bans";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; set; }

        public GetBansRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
