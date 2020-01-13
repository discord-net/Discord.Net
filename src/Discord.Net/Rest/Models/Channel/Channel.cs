using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.Models
{
    public class Channel
    {
        public const int MinChannelNameLength = 2;
        public const int MaxChannelNameLength = 100;

        public const int MinChannelTopicLength = 0;
        public const int MaxChannelTopicLength = 1024;

        public const int MinUserLimit = 0;
        public const int MaxUserLimit = 100;

        public const int MinBitrate = 8000;
        public const int MaxBitrate = 384000;

        public const int MinRateLimitPerUser = 0;
        public const int MaxRateLimitPerUser = 21600;

        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }
        [JsonPropertyName("type")]
        public ChannelType Type { get; set; }
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }
        [JsonPropertyName("position")]
        public Optional<short> Position { get; set; }
        [JsonPropertyName("permission_overwrites")]
        public Optional<Overwrite[]> Overwrites { get; set; }
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }
        [JsonPropertyName("topic")]
        public Optional<string?> Topic { get; set; }
        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; set; }
        [JsonPropertyName("user_limit")]
        public Optional<short> Bitrate { get; set; }
        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int> RateLimitPerUser { get; set; }
        [JsonPropertyName("recipients")]
        public Optional<User[]> Recipients { get; set; }
        [JsonPropertyName("icon")]
        public Optional<string?> IconId { get; set; }
        [JsonPropertyName("owner_id")]
        public Optional<Snowflake> OwnerId { get; set; }
        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; set; }
        [JsonPropertyName("parent_id")]
        public Optional<Snowflake> ParentId { get; set; }
        [JsonPropertyName("last_pin_timestamp")]
        public Optional<DateTimeOffset> LastPinTimestamp { get; set; }
        // omitted: last_message_id
    }
}
