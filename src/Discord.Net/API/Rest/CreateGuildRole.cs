using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class CreateRoleRequest : IRestRequest<Role>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public CreateRoleRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
