using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyGuildParams
    {
        /// <summary>
        /// Guild name.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        /// Guild voice region id (deprecated).
        /// </summary>
        public Optional<string?> Region { get; set; }

        /// <summary>
        /// Verification level.
        /// </summary>
        public Optional<int?> VerificationLevel { get; set; }

        /// <summary>
        /// Default message notification level.
        /// </summary>
        public Optional<int?> DefaultMessageNotifications { get; set; }

        /// <summary>
        /// Explicit content filter level.
        /// </summary>
        public Optional<int?> ExplicitContentFilter { get; set; }

        /// <summary>
        /// Id for afk channel.
        /// </summary>
        public Optional<Snowflake?> AfkChannelId { get; set; }

        /// <summary>
        /// Afk timeout in seconds.
        /// </summary>
        public Optional<int> AfkTimeout { get; set; }

        /// <summary>
        /// Image for the guild icon (can be animated gif when the server has the ANIMATED_ICON feature).
        /// </summary>
        public Optional<Image?> Icon { get; set; }

        /// <summary>
        /// User id to transfer guild ownership to (must be owner).
        /// </summary>
        public Optional<Snowflake> OwnerId { get; set; }

        /// <summary>
        /// Image for the guild splash (when the server has the INVITE_SPLASH feature).
        /// </summary>
        public Optional<Image?> Splash { get; set; }

        /// <summary>
        /// Image for the guild discovery splash (when the server has the DISCOVERABLE feature).
        /// </summary>
        public Optional<Image?> DiscoverySplash { get; set; }

        /// <summary>
        /// Image for the guild banner (when the server has the BANNER feature).
        /// </summary>
        public Optional<Image?> Banner { get; set; }

        /// <summary>
        /// The id of the channel where guild notices such as welcome messages and boost events are posted.
        /// </summary>
        public Optional<Snowflake?> SystemChannelId { get; set; }

        /// <summary>
        /// System channel flags.
        /// </summary>
        public Optional<int> SystemChannelFlags { get; set; }

        /// <summary>
        /// The id of the channel where Community guilds display rules and/or guidelines.
        /// </summary>
        public Optional<Snowflake?> RulesChannelId { get; set; }

        /// <summary>
        /// The id of the channel where admins and moderators of Community guilds receive notices from Discord.
        /// </summary>
        public Optional<Snowflake?> PublicUpdatesChannelId { get; set; }

        /// <summary>
        /// The preferred locale of a Community guild used in server discovery and notices from Discord; defaults to "en-US".
        /// </summary>
        public Optional<string?> PreferredLocale { get; set; }

        /// <summary>
        /// Enabled guild features.
        /// </summary>
        public Optional<string[]> Features { get; set; }

        /// <summary>
        /// The description for the guild, if the guild is discoverable.
        /// </summary>
        public Optional<string?> Description { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Name!, nameof(Name));
            Preconditions.LengthAtLeast(Name!, Guild.MinGuildNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name!, Guild.MaxGuildNameLength, nameof(Name));
        }
    }
}
