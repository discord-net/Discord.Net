using Discord.API;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal class InviteCreatedEvent
    {
        [JsonPropertyName("channel_id")]
        public ulong ChannelID { get; set; }
        [JsonPropertyName("code")]
        public string InviteCode { get; set; }
        [JsonPropertyName("timestamp")]
        public Optional<DateTimeOffset> RawTimestamp { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong? GuildID { get; set; }
        [JsonPropertyName("inviter")]
        public Optional<User> Inviter { get; set; }
        [JsonPropertyName("max_age")]
        public int RawAge { get; set; }
        [JsonPropertyName("max_uses")]
        public int MaxUsers { get; set; }
        [JsonPropertyName("temporary")]
        public bool TempInvite { get; set; }
        [JsonPropertyName("uses")]
        public int Uses { get; set; }
    }
}
