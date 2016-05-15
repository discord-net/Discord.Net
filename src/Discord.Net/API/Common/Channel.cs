using Newtonsoft.Json;

namespace Discord.API
{
    public class Channel
    {
        //Shared
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("last_message_id")]
        public ulong? LastMessageId { get; set; }

        //GuildChannel
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public ChannelType Type { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("permission_overwrites")]
        public Overwrite[] PermissionOverwrites { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }

        //DMChannel
        [JsonProperty("recipient")]
        public User Recipient { get; set; }
    }
}
