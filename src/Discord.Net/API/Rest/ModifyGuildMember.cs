using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyGuildMemberRequest : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/members/{UserId}";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }
        public ulong UserId { get; }

        [JsonProperty("roles")]
        public ulong[] Roles { get; set; }
        [JsonProperty("mute")]
        public bool Mute { get; set; }
        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }

        public ModifyGuildMemberRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}
