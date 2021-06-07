using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord invite metadata object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/invite#invite-metadata-object-invite-metadata-structure"/>
    /// </remarks>
    public record InviteMetadata : Invite
    {
        /// <summary>
        /// Number of times this <see cref="Invite"/> has been used.
        /// </summary>
        [JsonPropertyName("uses")]
        public int Uses { get; init; }

        /// <summary>
        /// Max number of times this <see cref="Invite"/> can be used.
        /// </summary>
        [JsonPropertyName("max_uses")]
        public int MaxUses { get; init; }

        /// <summary>
        /// Duration (in seconds) after which the <see cref="Invite"/> expires.
        /// </summary>
        [JsonPropertyName("max_age")]
        public int MaxAge { get; init; }

        /// <summary>
        /// Whether this <see cref="Invite"/> only grants temporary membership.
        /// </summary>
        [JsonPropertyName("temporary")]
        public bool Temporary { get; init; }

        /// <summary>
        /// When this <see cref="Invite"/> was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }
    }
}
