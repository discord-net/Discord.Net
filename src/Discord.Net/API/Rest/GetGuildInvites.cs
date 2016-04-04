using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildInvitesRequest : IRestRequest<Invite[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/invites";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildInvitesRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
