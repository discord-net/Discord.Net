using Discord.API.Converters;
using Newtonsoft.Json;
using System;

namespace Discord.API.Client
{
    public class Guild : GuildReference
    {
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

        //Unknown
        [JsonProperty("splash")]
        public object Splash { get; set; }
        [JsonProperty("features")]
        public object Features { get; set; }
        [JsonProperty("emojis")]
        public object Emojis { get; set; }
    }
}
