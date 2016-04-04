using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class DeleteGuildRoleRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles/{RoleId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }
        public ulong RoleId { get; }

        public DeleteGuildRoleRequest(ulong guildId, ulong roleId)
        {
            GuildId = guildId;
            RoleId = roleId;
        }
    }
}
