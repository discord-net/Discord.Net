using Discord.API.Converters;
using Newtonsoft.Json;
using System;

namespace Discord.API.Client
{
    public class Guild : GuildReference
    {
        public class EmojiData
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("roles"), JsonConverter(typeof(LongStringArrayConverter))]
            public ulong[] RoleIds { get; set; }
            [JsonProperty("require_colons")]
            public bool RequireColons { get; set; }
            [JsonProperty("managed")]
            public bool IsManaged { get; set; }
        }

        [JsonProperty("afk_channel_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? AFKChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int? AFKTimeout { get; set; }
        [JsonProperty("embed_channel_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? EmbedChannelId { get; set; }
        [JsonProperty("embed_enabled")]
        public bool EmbedEnabled { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("joined_at")]
        public DateTime? JoinedAt { get; set; }
        [JsonProperty("owner_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? OwnerId { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("roles")]
        public Role[] Roles { get; set; }
        [JsonProperty("features")]
        public string[] Features { get; set; }
        [JsonProperty("emojis")]
        public EmojiData[] Emojis { get; set; }
        [JsonProperty("splash")]
        public string Splash { get; set; }
        [JsonProperty("verification_level")]
        public int VerificationLevel { get; set; }
    }
}
