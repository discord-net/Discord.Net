using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord integration account object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#integration-account-object-integration-account-structure"/>
    /// </remarks>
    public record IntegrationAccount
    {
        /// <summary>
        /// Id of the account.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; init; } // Required property candidate

        /// <summary>
        /// Name of the account.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate
    }
}
