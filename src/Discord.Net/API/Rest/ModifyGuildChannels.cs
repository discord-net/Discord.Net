using Discord.Net.Rest;
using Newtonsoft.Json;
using System;

namespace Discord.API.Rest
{
    public class ModifyGuildChannelsRequest : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/channels";
        object IRestRequest.Payload => Requests;
        
        public ulong GuildId { get; }

        public ModifyGuildChannelRequest[] Requests { get; set; } = Array.Empty<ModifyGuildChannelRequest>();

        public ModifyGuildChannelsRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
