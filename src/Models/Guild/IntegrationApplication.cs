using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a integration application object.
    /// </summary>
    public record IntegrationApplication
    {
        /// <summary>
        ///     Creates a <see cref="IntegrationApplication"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The id of the app.</param>
        /// <param name="name">The name of the app.</param>
        /// <param name="icon">The icon hash of the app.</param>
        /// <param name="description">The description of the app.</param>
        /// <param name="summary">The description of the app.</param>
        /// <param name="bot">The bot associated with this application.</param>
        [JsonConstructor]
        public IntegrationApplication(Snowflake id, string name, string? icon, string description, string summary, Optional<User> bot)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Description = description;
            Summary = summary;
            Bot = bot;
        }

        /// <summary>
        ///     The id of the app.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The name of the app.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     The icon hash of the app.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; }

        /// <summary>
        ///     The description of the app.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; }

        /// <summary>
        ///     The description of the app.
        /// </summary>
        [JsonPropertyName("summary")]
        public string Summary { get; }

        /// <summary>
        ///     The bot associated with this application.
        /// </summary>
        [JsonPropertyName("bot")]
        public Optional<User> Bot { get; }
    }
}
