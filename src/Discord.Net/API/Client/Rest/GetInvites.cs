using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetInvitesRequest : IRestRequest<InviteReference[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/guilds/{GuildId}/invites";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; }

        public GetInvitesRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
