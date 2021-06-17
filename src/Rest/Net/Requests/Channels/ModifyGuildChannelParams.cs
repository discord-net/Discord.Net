using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to include in a request to modify a <see cref="GuildChannel"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#modify-channel-json-params-guild-channel"/>
    /// </remarks>
    public record ModifyGuildChannelParams
    {
        /// <summary>
        /// <see cref="Channel"/> name.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        /// The type of <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// Only conversion between text and news is supported and only in guilds with the "NEWS" feature.
        /// </remarks>
        public Optional<ChannelType> Type { get; set; }

        /// <summary>
        /// The position of the <see cref="Channel"/> in the left-hand listing.
        /// </summary>
        public Optional<int?> Position { get; set; }

        /// <summary>
        /// <see cref="Channel"/> topic.
        /// </summary>
        public Optional<string?> Topic { get; set; }

        /// <summary>
        /// Whether the <see cref="Channel"/> is nsfw.
        /// </summary>
        public Optional<bool?> Nsfw { get; set; }

        /// <summary>
        /// Amount of seconds a <see cref="User"/> has to wait before sending another message;
        /// bots, as well as <see cref="User"/>s with the permission <see cref="Permissions.ManageMessages"/>
        /// or <see cref="Permissions.ManageChannels"/>, are unaffected.
        /// </summary>
        public Optional<int?> RateLimitPerUser { get; set; }

        /// <summary>
        /// The bitrate (in bits) of the <see cref="VoiceChannel"/>.
        /// </summary>
        public Optional<int?> Bitrate { get; set; }

        /// <summary>
        /// The <see cref="User"/> limit of the <see cref="VoiceChannel"/>; 0 refers to no limit,
        /// 1 to 99 refers to a <see cref="User"/> limit.
        /// </summary>
        public Optional<int?> UserLimit { get; set; }

        /// <summary>
        /// <see cref="Channel"/> or category-specific permissions.
        /// </summary>
        public Optional<Overwrite[]?> PermissionOverwrites { get; set; }

        /// <summary>
        /// Id of the new parent <see cref="CategoryChannel"/> for a <see cref="Channel"/>.
        /// </summary>
        public Optional<Snowflake?> ParentId { get; set; }

        /// <summary>
        /// <see cref="Channel"/> voice region id, automatic when set to <see langword="null"/>.
        /// </summary>
        public Optional<string?> RtcRegion { get; set; }

        /// <summary>
        /// The camera video quality mode of the <see cref="VoiceChannel"/>.
        /// </summary>
        public Optional<int?> VideoQualityMode { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrWhitespace(Name!, nameof(Name));
            Preconditions.LengthAtLeast(Name!, Channel.MinChannelNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name!, Channel.MaxChannelNameLength, nameof(Name));
            Preconditions.MustBeOneOf(Type, new[] { ChannelType.GuildText, ChannelType.GuildNews }, nameof(Type));
            Preconditions.NotNegative(Position, nameof(Position));
            Preconditions.NotNull(Topic, nameof(Topic));
            Preconditions.LengthAtLeast(Topic, Channel.MinChannelTopicLength, nameof(Topic));
            Preconditions.LengthAtMost(Topic, Channel.MaxChannelTopicLength, nameof(Topic));
            Preconditions.AtLeast(RateLimitPerUser, Channel.MinRateLimitPerUserDuration, nameof(RateLimitPerUser));
            Preconditions.AtMost(RateLimitPerUser, Channel.MaxRateLimitPerUserDuration, nameof(RateLimitPerUser));
            Preconditions.AtLeast(Bitrate, Channel.MinBitrate, nameof(Bitrate));
            Preconditions.AtMost(Bitrate, Channel.MaxBitrate, nameof(Bitrate));
            Preconditions.AtLeast(UserLimit, Channel.MinUserLimit, nameof(UserLimit));
            Preconditions.AtMost(UserLimit, Channel.MaxUserLimit, nameof(UserLimit));
            Preconditions.NotNull(PermissionOverwrites, nameof(PermissionOverwrites));
        }
    }
}
