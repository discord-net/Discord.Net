using Discord.Net.Rest;
using System;

namespace Discord.API.Rest
{
    public class ModifyGuildRolesRequest : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles";
        object IRestRequest.Payload => Requests;
        
        public ulong GuildId { get; }

        public ModifyGuildRoleRequest[] Requests { get; set; } = Array.Empty<ModifyGuildRoleRequest>();

        public ModifyGuildRolesRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
