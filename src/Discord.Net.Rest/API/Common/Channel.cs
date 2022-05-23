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
        [JsonProperty("parent_id")]
        public ulong? CategoryId { get; set; }

        //TextChannel
        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }
        [JsonProperty("last_pin_timestamp")]
        public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }
        [JsonProperty("nsfw")]
        public Optional<bool> Nsfw { get; set; }
        [JsonProperty("rate_limit_per_user")]
        public Optional<int> SlowMode { get; set; }

        //VoiceChannel
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }
        [JsonProperty("rtc_region")]
        public Optional<string> RTCRegion { get; set; }

        //PrivateChannel
        [JsonProperty("recipients")]
        public Optional<User[]> Recipients { get; set; }

        //GroupChannel
        [JsonProperty("icon")]
        public Optional<string> Icon { get; set; }

        //ThreadChannel
        [JsonProperty("member")]
        public Optional<ThreadMember> ThreadMember { get; set; }

        [JsonProperty("thread_metadata")]
        public Optional<ThreadMetadata> ThreadMetadata { get; set; }

        [JsonProperty("owner_id")]
        public Optional<ulong> OwnerId { get; set; }

        [JsonProperty("message_count")]
        public Optional<int> MessageCount { get; set; }

        [JsonProperty("member_count")]
        public Optional<int> MemberCount { get; set; }

        //ForumChannel
        [JsonProperty("available_tags")]
        public Optional<ForumTags[]> ForumTags { get; set; }
        
        [JsonProperty("default_auto_archive_duration")]
        public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }
    }
}
