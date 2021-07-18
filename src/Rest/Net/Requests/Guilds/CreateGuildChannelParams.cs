using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateGuildChannelParams
    {
        /// <summary>
        /// Channel name (1-100 characters).
        /// </summary>
        public string? Name { get; set; } // Required property candidate

        /// <summary>
        /// The type of channel.
        /// </summary>
        public Optional<ChannelType> Type { get; set; }

        /// <summary>
        /// Channel topic (0-1024 characters).
        /// </summary>
        public Optional<string> Topic { get; set; }

        /// <summary>
        /// The bitrate (in bits) of the voice channel (voice only).
        /// </summary>
        public Optional<int> Bitrate { get; set; }

        /// <summary>
        /// The user limit of the voice channel (voice only).
        /// </summary>
        public Optional<int> UserLimit { get; set; }

        /// <summary>
        /// Amount of seconds a user has to wait before sending another message (0-21600);
        /// bots, as well as users with the permission <see cref="Permissions.ManageMessages"/> or
        /// <see cref="Permissions.ManageChannels"/>, are unaffected.
        /// </summary>
        public Optional<int> RateLimitPerUser { get; set; }

        /// <summary>
        /// Sorting position of the channel.
        /// </summary>
        public Optional<int> Position { get; set; }

        /// <summary>
        /// The channel's permission overwrites.
        /// </summary>
        public Optional<Overwrite[]> PermissionOverwrites { get; set; }

        /// <summary>
        /// Id of the parent category for a channel.
        /// </summary>
        public Optional<Snowflake> ParentId { get; set; }

        /// <summary>
        /// Whether the channel is nsfw.
        /// </summary>
        public Optional<bool> Nsfw { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Name, nameof(Name));
            Preconditions.LengthAtLeast(Name, Channel.MinChannelNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name, Channel.MaxChannelNameLength, nameof(Name));
            Preconditions.NotNegative(Position, nameof(Position));
            Preconditions.NotNull(Topic!, nameof(Topic));
            Preconditions.LengthAtLeast(Topic!, Channel.MinChannelTopicLength, nameof(Topic));
            Preconditions.LengthAtMost(Topic!, Channel.MaxChannelTopicLength, nameof(Topic));
            Preconditions.AtLeast(RateLimitPerUser, Channel.MinRateLimitPerUserDuration, nameof(RateLimitPerUser));
            Preconditions.AtMost(RateLimitPerUser, Channel.MaxRateLimitPerUserDuration, nameof(RateLimitPerUser));
            Preconditions.AtLeast(Bitrate, Channel.MinBitrate, nameof(Bitrate));
            Preconditions.AtMost(Bitrate, Channel.MaxBitrate, nameof(Bitrate));
            Preconditions.AtLeast(UserLimit, Channel.MinUserLimit, nameof(UserLimit));
            Preconditions.AtMost(UserLimit, Channel.MaxUserLimit, nameof(UserLimit));
            Preconditions.NotNull(PermissionOverwrites!, nameof(PermissionOverwrites));
        }
    }
}
