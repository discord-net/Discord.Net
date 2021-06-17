using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateChannelInviteParams
    {
        /// <summary>
        /// Duration of <see cref="Invite"/> in seconds before expiry.
        /// </summary>
        /// <remarks>
        /// Must be between 0 and 604800 (7 days), inclusive. Zero means never.
        /// </remarks>
        public Optional<int> MaxAge { get; set; }

        /// <summary>
        /// Maximum number of uses.
        /// </summary>
        /// <remarks>
        /// Must be between 0 and 100, inclusive. Zero means unlimited.
        /// </remarks>
        public Optional<int> MaxUses { get; set; }

        /// <summary>
        /// Whether this <see cref="Invite"/> only grants temporary membership.
        /// </summary>
        public Optional<bool> Temporary { get; set; }

        /// <summary>
        /// If true, don't try to reuse a similar <see cref="Invite"/> (useful for
        /// creating many unique one time use invites).
        /// </summary>
        public Optional<bool> Unique { get; set; }

        /// <summary>
        /// The type of target for this <see cref="VoiceChannel"/> <see cref="Invite"/>.
        /// </summary>
        public Optional<InviteTargetType> TargetType { get; set; }

        /// <summary>
        /// The id of the <see cref="User"/> whose stream to display for this <see cref="Invite"/>.
        /// </summary>
        /// <remarks>
        /// Required if <see cref="TargetType"/> is <see cref="InviteTargetType.Stream"/>,
        /// the <see cref="User"/> must be streaming in the <see cref="Channel"/>.
        /// </remarks>
        public Optional<Snowflake> TargetUserId { get; set; }

        /// <summary>
        /// The id of the embedded application to open for this <see cref="Invite"/>.
        /// </summary>
        /// <remarks>
        /// Required if  <see cref="TargetType"/> is <see cref="InviteTargetType.EmbeddedApplication"/>,
        /// the application must have the <see cref="ApplicationFlags.Embedded"/> flag.
        /// </remarks>
        public Optional<Snowflake> TargetApplicationId { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.AtLeast(MaxAge, InviteWithMetadata.MinAgeTime, nameof(MaxAge));
            Preconditions.AtMost(MaxAge, InviteWithMetadata.MaxAgeTime, nameof(MaxAge));
            Preconditions.AtLeast(MaxUses, InviteWithMetadata.MinUseCount, nameof(MaxUses));
            Preconditions.AtMost(MaxUses, InviteWithMetadata.MaxUseCount, nameof(MaxUses));
            Preconditions.NotZero(TargetUserId, nameof(TargetUserId));
            Preconditions.NotZero(TargetApplicationId, nameof(TargetApplicationId));
        }
    }
}
