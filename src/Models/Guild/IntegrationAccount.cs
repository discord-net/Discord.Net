using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a integration account object.
    /// </summary>
    public record IntegrationAccount
    {
        /// <summary>
        ///     Creates a <see cref="IntegrationAccount"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Id of the account.</param>
        /// <param name="name">Name of the account.</param>
        [JsonConstructor]
        public IntegrationAccount(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Id of the account.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        ///     Name of the account.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }
    }
}
