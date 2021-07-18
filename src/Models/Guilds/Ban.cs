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
        /// Minimum amount of days to delete messages when banning.
        /// </summary>
        public const int MinDaysToDeleteMessages = 0;

        /// <summary>
        /// Maximum amount of days to delete messages when banning.
        /// </summary>
        public const int MaxDaysToDeleteMessages = 7;

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
