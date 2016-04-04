using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildRolesRequest : IRestRequest<Role[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guild/{GuildId}/roles";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildRolesRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
