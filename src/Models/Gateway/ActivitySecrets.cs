using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an activity secrets object.
    /// </summary>
    public record ActivitySecrets
    {
        /// <summary>
        ///     Creates a <see cref="ActivitySecrets"/> with the provided parameters.
        /// </summary>
        /// <param name="join">The secret for joining a party.</param>
        /// <param name="spectate">The secret for spectating a game.</param>
        /// <param name="match">The secret for a specific instanced match.</param>
        [JsonConstructor]
        public ActivitySecrets(Optional<string> join, Optional<string> spectate, Optional<string> match)
        {
            Join = join;
            Spectate = spectate;
            Match = match;
        }

        /// <summary>
        ///     The secret for joining a party.
        /// </summary>
        [JsonPropertyName("join")]
        public Optional<string> Join { get; }

        /// <summary>
        ///     The secret for spectating a game.
        /// </summary>
        [JsonPropertyName("spectate")]
        public Optional<string> Spectate { get; }

        /// <summary>
        ///     The secret for a specific instanced match.
        /// </summary>
        [JsonPropertyName("match")]
        public Optional<string> Match { get; }
    }
}
