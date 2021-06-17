using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord channel object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#channel-object-channel-structure"/>
    /// </remarks>
    public record Channel
    {
        /// <summary>
        /// Minimum langth of a channel name.
        /// </summary>
        public const int MinChannelNameLength = 1;

        /// <summary>
        /// Maximum langth of a channel name.
        /// </summary>
        public const int MaxChannelNameLength = 100;

        /// <summary>
        /// Minimum langth of a channel topic.
        /// </summary>
        public const int MinChannelTopicLength = 0;

        /// <summary>
        /// Maximum langth of a channel topic.
        /// </summary>
        public const int MaxChannelTopicLength = 1024;

        /// <summary>
        /// Minimum langth of a channel topic.
        /// </summary>
        public const int MinRateLimitPerUserDuration = 0;

        /// <summary>
        /// Maximum langth of a channel topic.
        /// </summary>
        public const int MaxRateLimitPerUserDuration = 1024;

        /// <summary>
        /// Minimum amount of users in channel.
        /// </summary>
        public const int MinUserLimit = 0;

        /// <summary>
        /// Maximum amount of users in channel.
        /// </summary>
        public const int MaxUserLimit = 99;

        /// <summary>
        /// Minimum bitrate for a channel.
        /// </summary>
        public const int MinBitrate = 8000;

        /// <summary>
        /// Maximum bitrate for a channel.
        /// </summary>
        public const int MaxBitrate = 128000;

        /// <summary>
        /// Minimum afk timeout duration for a channel.
        /// </summary>
        public const int MinAfkTimeoutDuration = 60;

        /// <summary>
        /// Maximum afk timeout duration for a channel.
        /// </summary>
        public const int MaxAFkTimeoutDuration = 3600;

        /// <summary>
        /// Minimum amount of messages to requests in a channel.
        /// </summary>
        public const int MinGetMessagesAmount = 1;

        /// <summary>
        /// Maximum amount of messages to requests in a channel.
        /// </summary>
        public const int MaxGetMessagesAmount = 100;

        /// <summary>
        /// The id of this <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// The type of <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("type")]
        public ChannelType Type { get; init; }

        /// <summary>
        /// The id of the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// May be missing for some channel objects received over gateway guild dispatches.
        /// </remarks>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; init; }

        /// <summary>
        /// Sorting position of the <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("position")]
        public Optional<int> Position { get; init; }

        /// <summary>
        /// Explicit permission <see cref="Overwrite"/>s for <see cref="GuildMember"/>s
        /// and <see cref="Role"/>s.
        /// </summary>
        [JsonPropertyName("permission_overwrites")]
        public Optional<Overwrite[]> PermissionOverwrites { get; init; }

        /// <summary>
        /// The name of the <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public Optional<string> Name { get; init; }

        /// <summary>
        /// The <see cref="Channel"/> topic.
        /// </summary>
        [JsonPropertyName("topic")]
        public Optional<string?> Topic { get; init; }

        /// <summary>
        /// Whether the <see cref="Channel"/> is nsfw.
        /// </summary>
        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; init; }

        /// <summary>
        /// The id of the last <see cref="Message"/> sent in this <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// May not point to an existing or valid message.
        /// </remarks>
        [JsonPropertyName("last_message_id")]
        public Optional<Snowflake?> LastMessageId { get; init; }

        /// <summary>
        /// The bitrate (in bits) of the <see cref="VoiceChannel"/>.
        /// </summary>
        [JsonPropertyName("bitrate")]
        public Optional<int> Bitrate { get; init; }

        /// <summary>
        /// The <see cref="User"/> limit of the <see cref="VoiceChannel"/>.
        /// </summary>
        [JsonPropertyName("user_limit")]
        public Optional<int> UserLimit { get; init; }

        /// <summary>
        /// Amount of seconds a <see cref="User"/> has to wait before sending another <see cref="Message"/>;
        /// bots, as well as <see cref="User"/>s with the permission <see cref="Permissions.ManageMessages"/>
        /// or <see cref="Permissions.ManageChannels"/>, are unaffected.
        /// </summary>
        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int> RateLimitPerUser { get; init; }

        /// <summary>
        /// The recipients of the DM.
        /// </summary>
        [JsonPropertyName("recipients")]
        public Optional<User[]> Recipients { get; init; }

        /// <summary>
        /// Icon hash.
        /// </summary>
        [JsonPropertyName("icon")]
        public Optional<string?> Icon { get; init; }

        /// <summary>
        /// Id of the creator of the group DM or thread.
        /// </summary>
        [JsonPropertyName("owner_id")]
        public Optional<Snowflake> OwnerId { get; init; }

        /// <summary>
        /// <see cref="Application"/> id of the group DM creator if it is bot-created.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; init; }

        /// <summary>
        /// For guild channels: id of the parent category for a channel (each parent
        /// category can contain up to 50 channels), for threads: id of the text channel
        /// this thread was created.
        /// </summary>
        [JsonPropertyName("parent_id")]
        public Optional<Snowflake?> ParentId { get; init; }

        /// <summary>
        /// When the last pinned <see cref="Message"/> was pinned.
        /// </summary>
        /// <remarks>
        /// This may be null in events such
        /// as GUILD_CREATE when a message is not pinned.
        /// </remarks>
        [JsonPropertyName("last_pin_timestamp")]
        public Optional<DateTimeOffset?> LastPinTimestamp { get; init; }

        /// <summary>
        /// Voice region id for the <see cref="VoiceChannel"/>, automatic when set to null.
        /// </summary>
        [JsonPropertyName("rtc_region")]
        public Optional<string?> RtcRegion { get; init; }

        /// <summary>
        /// The camera video quality mode of the <see cref="VoiceChannel"/>, 1 when not present.
        /// </summary>
        [JsonPropertyName("video_quality_mode")]
        public Optional<VideoQualityMode> VideoQualityMode { get; init; }

        /// <summary>
        /// An approximate count of <see cref="Message"/>s in a thread, stops counting at 50.
        /// </summary>
        [JsonPropertyName("message_count")]
        public Optional<int> MessageCount { get; init; }

        /// <summary>
        /// An approximate count of <see cref="User"/>s in a thread, stops counting at 50.
        /// </summary>
        [JsonPropertyName("member_count")]
        public Optional<int> MemberCount { get; init; }

        /// <summary>
        /// Thread-specific fields not needed by other channels.
        /// </summary>
        [JsonPropertyName("thread_metadata")]
        public Optional<ThreadMetadata> ThreadMetadata { get; init; }

        /// <summary>
        /// Thread member object for the current user, if they have joined the
        /// thread, only included on certain API endpoints.
        /// </summary>
        [JsonPropertyName("member")]
        public Optional<ThreadMember> Member { get; init; }
    }
}
