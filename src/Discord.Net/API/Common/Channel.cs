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
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("type")]
        public Optional<ChannelType> Type { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }
        [JsonProperty("permission_overwrites")]
        public Optional<Overwrite[]> PermissionOverwrites { get; set; }

        //TextChannel
        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }

        //VoiceChannel
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }

        //DMChannel
        [JsonProperty("recipient")]
        public Optional<User> Recipient { get; set; }
    }
}
