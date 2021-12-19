using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal class InviteCreatedEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        [JsonProperty("code")]
        public string InviteCode { get; set; }
        [JsonProperty("timestamp")]
        public Optional<DateTimeOffset> RawTimestamp { get; set; }
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }
        [JsonProperty("inviter")]
        public Optional<User> Inviter { get; set; }
        [JsonProperty("max_age")]
        public int RawAge { get; set; }
        [JsonProperty("max_uses")]
        public int MaxUsers { get; set; }
        [JsonProperty("temporary")]
        public bool TempInvite { get; set; }
        [JsonProperty("uses")]
        public int Uses { get; set; }
    }
}
