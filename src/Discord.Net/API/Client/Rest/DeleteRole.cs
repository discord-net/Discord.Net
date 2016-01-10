using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class DeleteRoleRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles/{RoleId}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }

        public DeleteRoleRequest(ulong guildId, ulong roleId)
        {
            GuildId = guildId;
            RoleId = roleId;
        }
    }
}
