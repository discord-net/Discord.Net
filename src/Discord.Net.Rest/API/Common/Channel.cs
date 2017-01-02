#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class Channel
    {
        //Shared
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public ChannelType Type { get; set; }
        [JsonProperty("last_message_id")]
        public ulong? LastMessageId { get; set; }

        //GuildChannel
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }
        [JsonProperty("permission_overwrites")]
        public Optional<Overwrite[]> PermissionOverwrites { get; set; }

        //TextChannel
        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }
        [JsonProperty("last_pin_timestamp")]
        public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }

        //VoiceChannel
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }

        //PrivateChannel
        [JsonProperty("recipients")]
        public Optional<User[]> Recipients { get; set; }

        //GroupChannel
        [JsonProperty("icon")]
        public Optional<string> Icon { get; set; }
    }
}
