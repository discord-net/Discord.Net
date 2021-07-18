using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateGuildParams
    {
        /// <summary>
        /// Name of the guild (2-100 characters).
        /// </summary>
        public string? Name { get; set; } // Required property candidate

        /// <summary>
        /// Image for the guild icon.
        /// </summary>
        public Optional<Image> Icon { get; set; }

        /// <summary>
        /// Verification level.
        /// </summary>
        public Optional<VerificationLevel> VerificationLevel { get; set; }

        /// <summary>
        /// Default message notification level.
        /// </summary>
        public Optional<DefaultMessageNotificationLevel> DefaultMessageNotifications { get; set; }

        /// <summary>
        /// Explicit content filter level.
        /// </summary>
        public Optional<ExplicitContentFilterLevel> ExplicitContentFilter { get; set; }

        /// <summary>
        /// New guild roles.
        /// </summary>
        public Optional<Role[]> Roles { get; set; }

        /// <summary>
        /// New guild's channels.
        /// </summary>
        public Optional<Channel[]> Channels { get; set; }

        /// <summary>
        /// Id for afk channel.
        /// </summary>
        public Optional<Snowflake> AfkChannelId { get; set; }

        /// <summary>
        /// Afk timeout in seconds.
        /// </summary>
        public Optional<int> AfkTimeout { get; set; }

        /// <summary>
        /// The id of the channel where guild notices such as welcome messages and boost events are posted.
        /// </summary>
        public Optional<Snowflake> SystemChannelId { get; set; }

        /// <summary>
        /// System channel flags.
        /// </summary>
        public Optional<SystemChannelFlags> SystemChannelFlags { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Name, nameof(Name));
            Preconditions.LengthAtLeast(Name, Guild.MinGuildNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name, Guild.MaxGuildNameLength, nameof(Name));
        }
    }
}
