using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UpdateRoleRequest : IRestRequest<Role>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles/{RoleId}";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("permissions")]
        public uint Permissions { get; set; }
        [JsonProperty("hoist")]
        public bool IsHoisted { get; set; }
        [JsonProperty("color")]
        public uint Color { get; set; }

        public UpdateRoleRequest(ulong guildId, ulong roleId)
        {
            GuildId = guildId;
            RoleId = roleId;
        }
    }
}
