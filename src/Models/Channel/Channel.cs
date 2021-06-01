using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a channel object.
    /// </summary>
    public record Channel
    {
        /// <summary>
        ///     Creates a <see cref="Channel"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The id of this channel.</param>
        /// <param name="type">The type of channel.</param>
        /// <param name="guildId">The id of the guild (may be missing for some channel objects received over gateway guild dispatches).</param>
        /// <param name="position">Sorting position of the channel.</param>
        /// <param name="permissionOverwrites">Explicit permission overwrites for members and roles.</param>
        /// <param name="name">The name of the channel (2-100 characters).</param>
        /// <param name="topic">The channel topic (0-1024 characters).</param>
        /// <param name="nsfw">Whether the channel is nsfw.</param>
        /// <param name="lastMessageId">The id of the last message sent in this channel (may not point to an existing or valid message).</param>
        /// <param name="bitrate">The bitrate (in bits) of the voice channel.</param>
        /// <param name="userLimit">The user limit of the voice channel.</param>
        /// <param name="rateLimitPerUser">Amount of seconds a user has to wait before sending another message (0-21600); bots, as well as users with the permission manage_messages or manage_channel, are unaffected.</param>
        /// <param name="recipients">The recipients of the DM.</param>
        /// <param name="icon">Icon hash.</param>
        /// <param name="ownerId">Id of the creator of the group DM or thread.</param>
        /// <param name="applicationId">Application id of the group DM creator if it is bot-created.</param>
        /// <param name="parentId">For guild channels: id of the parent category for a channel (each parent category can contain up to 50 channels), for threads: id of the text channel this thread was created.</param>
        /// <param name="lastPinTimestamp">When the last pinned message was pinned. This may be null in events such as GUILD_CREATE when a message is not pinned.</param>
        /// <param name="rtcRegion">Voice region id for the voice channel, automatic when set to null.</param>
        /// <param name="videoQualityMode">The camera video quality mode of the voice channel, 1 when not present.</param>
        /// <param name="messageCount">An approximate count of messages in a thread, stops counting at 50.</param>
        /// <param name="memberCount">An approximate count of users in a thread, stops counting at 50.</param>
        /// <param name="threadMetadata">Thread-specific fields not needed by other channels.</param>
        /// <param name="member">Thread member object for the current user, if they have joined the thread, only included on certain API endpoints.</param>
        [JsonConstructor]
        public Channel(Snowflake id, ChannelType type, Optional<Snowflake> guildId, Optional<int> position, Optional<Overwrite[]> permissionOverwrites, Optional<string> name, Optional<string?> topic, Optional<bool> nsfw, Optional<Snowflake?> lastMessageId, Optional<int> bitrate, Optional<int> userLimit, Optional<int> rateLimitPerUser, Optional<User[]> recipients, Optional<string?> icon, Optional<Snowflake> ownerId, Optional<Snowflake> applicationId, Optional<Snowflake?> parentId, Optional<DateTimeOffset?> lastPinTimestamp, Optional<string?> rtcRegion, Optional<VideoQualityMode> videoQualityMode, Optional<int> messageCount, Optional<int> memberCount, Optional<ThreadMetadata> threadMetadata, Optional<ThreadMember> member)
        {
            Id = id;
            Type = type;
            GuildId = guildId;
            Position = position;
            PermissionOverwrites = permissionOverwrites;
            Name = name;
            Topic = topic;
            Nsfw = nsfw;
            LastMessageId = lastMessageId;
            Bitrate = bitrate;
            UserLimit = userLimit;
            RateLimitPerUser = rateLimitPerUser;
            Recipients = recipients;
            Icon = icon;
            OwnerId = ownerId;
            ApplicationId = applicationId;
            ParentId = parentId;
            LastPinTimestamp = lastPinTimestamp;
            RtcRegion = rtcRegion;
            VideoQualityMode = videoQualityMode;
            MessageCount = messageCount;
            MemberCount = memberCount;
            ThreadMetadata = threadMetadata;
            Member = member;
        }

        /// <summary>
        ///     The id of this channel.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The type of channel.
        /// </summary>
        [JsonPropertyName("type")]
        public ChannelType Type { get; }

        /// <summary>
        ///     The id of the guild (may be missing for some channel objects received over gateway guild dispatches).
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; }

        /// <summary>
        ///     Sorting position of the channel.
        /// </summary>
        [JsonPropertyName("position")]
        public Optional<int> Position { get; }

        /// <summary>
        ///     Explicit permission overwrites for members and roles.
        /// </summary>
        [JsonPropertyName("permission_overwrites")]
        public Optional<Overwrite[]> PermissionOverwrites { get; }

        /// <summary>
        ///     The name of the channel (2-100 characters).
        /// </summary>
        [JsonPropertyName("name")]
        public Optional<string> Name { get; }

        /// <summary>
        ///     The channel topic (0-1024 characters).
        /// </summary>
        [JsonPropertyName("topic")]
        public Optional<string?> Topic { get; }

        /// <summary>
        ///     Whether the channel is nsfw.
        /// </summary>
        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; }

        /// <summary>
        ///     The id of the last message sent in this channel (may not point to an existing or valid message).
        /// </summary>
        [JsonPropertyName("last_message_id")]
        public Optional<Snowflake?> LastMessageId { get; }

        /// <summary>
        ///     The bitrate (in bits) of the voice channel.
        /// </summary>
        [JsonPropertyName("bitrate")]
        public Optional<int> Bitrate { get; }

        /// <summary>
        ///     The user limit of the voice channel.
        /// </summary>
        [JsonPropertyName("user_limit")]
        public Optional<int> UserLimit { get; }

        /// <summary>
        ///     Amount of seconds a user has to wait before sending another message (0-21600); bots, as well as users with the permission manage_messages or manage_channel, are unaffected.
        /// </summary>
        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int> RateLimitPerUser { get; }

        /// <summary>
        ///     The recipients of the DM.
        /// </summary>
        [JsonPropertyName("recipients")]
        public Optional<User[]> Recipients { get; }

        /// <summary>
        ///     Icon hash.
        /// </summary>
        [JsonPropertyName("icon")]
        public Optional<string?> Icon { get; }

        /// <summary>
        ///     Id of the creator of the group DM or thread.
        /// </summary>
        [JsonPropertyName("owner_id")]
        public Optional<Snowflake> OwnerId { get; }

        /// <summary>
        ///     Application id of the group DM creator if it is bot-created.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; }

        /// <summary>
        ///     For guild channels: id of the parent category for a channel (each parent category can contain up to 50 channels), for threads: id of the text channel this thread was created.
        /// </summary>
        [JsonPropertyName("parent_id")]
        public Optional<Snowflake?> ParentId { get; }

        /// <summary>
        ///     When the last pinned message was pinned. This may be null in events such as GUILD_CREATE when a message is not pinned.
        /// </summary>
        [JsonPropertyName("last_pin_timestamp")]
        public Optional<DateTimeOffset?> LastPinTimestamp { get; }

        /// <summary>
        ///     Voice region id for the voice channel, automatic when set to null.
        /// </summary>
        [JsonPropertyName("rtc_region")]
        public Optional<string?> RtcRegion { get; }

        /// <summary>
        ///     The camera video quality mode of the voice channel, 1 when not present.
        /// </summary>
        [JsonPropertyName("video_quality_mode")]
        public Optional<VideoQualityMode> VideoQualityMode { get; }

        /// <summary>
        ///     An approximate count of messages in a thread, stops counting at 50.
        /// </summary>
        [JsonPropertyName("message_count")]
        public Optional<int> MessageCount { get; }

        /// <summary>
        ///     An approximate count of users in a thread, stops counting at 50.
        /// </summary>
        [JsonPropertyName("member_count")]
        public Optional<int> MemberCount { get; }

        /// <summary>
        ///     Thread-specific fields not needed by other channels.
        /// </summary>
        [JsonPropertyName("thread_metadata")]
        public Optional<ThreadMetadata> ThreadMetadata { get; }

        /// <summary>
        ///     Thread member object for the current user, if they have joined the thread, only included on certain API endpoints.
        /// </summary>
        [JsonPropertyName("member")]
        public Optional<ThreadMember> Member { get; }
    }
}
