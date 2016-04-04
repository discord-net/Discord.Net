using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildRoleRequest : IRestRequest<Role>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles/{RoleId}";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }
        public ulong RoleId { get; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("permissions")]
        public int Permissions { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
        [JsonProperty("hoist")]
        public bool Hoist { get; set; }

        public ModifyGuildRoleRequest(ulong guildId, ulong roleId)
        {
            GuildId = guildId;
            RoleId = roleId;
        }
    }
}
