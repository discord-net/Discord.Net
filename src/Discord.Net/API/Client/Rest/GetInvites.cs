using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetInvitesRequest : IRestRequest<InviteReference[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/invites";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; set; }

        public GetInvitesRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
