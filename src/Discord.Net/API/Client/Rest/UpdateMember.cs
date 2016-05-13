using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateMemberRequest : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/members/{UserId}";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }

        [JsonProperty("mute")]
        public bool IsMuted { get; set; }
        [JsonProperty("deaf")]
        public bool IsDeafened { get; set; }
        [JsonProperty("channel_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? VoiceChannelId { get; set; }
        [JsonProperty("roles"), JsonConverter(typeof(LongStringArrayConverter))]
        public ulong[] RoleIds { get; set; }
        [JsonProperty("nick")]
        public string Nickname { get; set; }

        public UpdateMemberRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}
