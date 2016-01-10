using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetBansRequest : IRestRequest<UserReference[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/bans";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; set; }

        public GetBansRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
