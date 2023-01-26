using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildChannelParams
    {
        [JsonProperty("name")]
        public string Name { get; }
        [JsonProperty("type")]
        public ChannelType Type { get; }
        [JsonProperty("parent_id")]
        public Optional<ulong?> CategoryId { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }
        [JsonProperty("permission_overwrites")]
        public Optional<Overwrite[]> Overwrites { get; set; }

        //Text channels
        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }
        [JsonProperty("nsfw")]
        public Optional<bool> IsNsfw { get; set; }
        [JsonProperty("rate_limit_per_user")]
        public Optional<int> SlowModeInterval { get; set; }
        [JsonProperty("default_auto_archive_duration")]
        public Optional<ThreadArchiveDuration> DefaultAutoArchiveDuration { get; set; }

        //Voice channels
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int?> UserLimit { get; set; }
        [JsonProperty("video_quality_mode")]
        public Optional<VideoQualityMode> VideoQuality { get; set; }
        [JsonProperty("rtc_region")]
        public Optional<string> RtcRegion { get; set; }

        //Forum channels
        [JsonProperty("default_reaction_emoji")]
        public Optional<ModifyForumReactionEmojiParams> DefaultReactionEmoji { get; set; }
        [JsonProperty("default_thread_rate_limit_per_user")]
        public Optional<int> ThreadRateLimitPerUser { get; set; }
        [JsonProperty("available_tags")]
        public Optional<ModifyForumTagParams[]> AvailableTags { get; set; }
        [JsonProperty("default_sort_order")]
        public Optional<ForumSortOrder?> DefaultSortOrder { get; set; }

        public CreateGuildChannelParams(string name, ChannelType type)
        {
            Name = name;
            Type = type;
        }
    }
}
