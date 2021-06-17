using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to include in a request to modify a <see cref="ThreadChannel"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#modify-channel-json-params-guild-channel"/>
    /// </remarks>
    public record ModifyThreadChannelParams
    {
        /// <summary>
        /// <see cref="ThreadChannel"/> name.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        /// Whether the <see cref="ThreadChannel"/> is archived.
        /// </summary>
        public Optional<bool> Archived { get; set; }

        /// <summary>
        /// Duration in minutes to automatically archive the <see cref="ThreadChannel"/> after recent activity.
        /// </summary>
        /// <remarks>
        /// Can be set to 60, 1440, 4320, or 10080.
        /// </remarks>
        public Optional<int> AutoArchiveDuration { get; set; }

        /// <summary>
        /// When a <see cref="ThreadChannel"/> is locked, only <see cref="User"/>s with
        /// <see cref="Permissions.ManageThreads"/> can unarchive it.
        /// </summary>
        public Optional<bool> Locked { get; set; }

        /// <summary>
        /// Amount of seconds a <see cref="User"/> has to wait before sending another message;
        /// bots, as well as <see cref="User"/>s with the permission <see cref="Permissions.ManageMessages"/>,
        /// <see cref="Permissions.ManageThreads"/>, or <see cref="Permissions.ManageChannels"/>, are unaffected.
        /// </summary>
        public Optional<int?> RateLimitPerUser { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrWhitespace(Name!, nameof(Name));
            Preconditions.LengthAtLeast(Name!, Channel.MinChannelNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name!, Channel.MaxChannelNameLength, nameof(Name));
            Preconditions.MustBeOneOf(AutoArchiveDuration, new[] { 60, 1440, 4320, 10080 }, nameof(AutoArchiveDuration));
            Preconditions.AtLeast(RateLimitPerUser, Channel.MinRateLimitPerUserDuration, nameof(RateLimitPerUser));
            Preconditions.AtMost(RateLimitPerUser, Channel.MaxRateLimitPerUserDuration, nameof(RateLimitPerUser));
        }
    }
}
