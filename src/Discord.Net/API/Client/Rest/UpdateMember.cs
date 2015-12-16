using Discord.API.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UpdateMemberRequest : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/members/{UserId}";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; }
        public ulong UserId { get; }

        [JsonProperty("mute")]
        public bool IsMuted { get; set; }
        [JsonProperty("deaf")]
        public bool IsDeafened { get; set; }
        [JsonProperty("channel_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? VoiceChannelId { get; set; }
        [JsonProperty("roles"), JsonConverter(typeof(LongStringArrayConverter))]
        public ulong[] RoleIds { get; set; }

        public UpdateMemberRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}
