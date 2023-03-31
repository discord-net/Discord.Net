using Newtonsoft.Json;
using System;

namespace Discord.API.Gateway
{
    internal class InviteCreateEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("inviter")]
        public Optional<User> Inviter { get; set; }
        [JsonProperty("max_age")]
        public int MaxAge { get; set; }
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }
        [JsonProperty("target_user")]
        public Optional<User> TargetUser { get; set; }
        [JsonProperty("target_type")]
        public Optional<TargetUserType> TargetUserType { get; set; }
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        [JsonProperty("uses")]
        public int Uses { get; set; }

        [JsonProperty("target_application")]
        public Optional<Application> Application { get; set; }

        [JsonProperty("expires_at")]
        public Optional<DateTimeOffset?> ExpiresAt { get; set; }
    }
}
