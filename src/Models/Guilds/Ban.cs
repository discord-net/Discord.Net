using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord ban object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#ban-object-ban-structure"/>
    /// </remarks>
    public record Ban
    {
        /// <summary>
        /// The reason for the ban.
        /// </summary>
        [JsonPropertyName("reason")]
        public string? Reason { get; init; }

        /// <summary>
        /// The banned <see cref="Models.User"/>.
        /// </summary>
        [JsonPropertyName("user")]
        public User? User { get; init; }  // Required property candidate
    }
}
