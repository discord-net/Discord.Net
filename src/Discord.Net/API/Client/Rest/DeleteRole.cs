using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DeleteRoleRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles/{RoleId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }

        public DeleteRoleRequest(ulong guildId, ulong roleId)
        {
            GuildId = guildId;
            RoleId = roleId;
        }
    }
}
