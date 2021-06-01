using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a message interaction object.
    /// </summary>
    public record MessageInteraction
    {
        /// <summary>
        ///     Creates a <see cref="MessageInteraction"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Id of the interaction.</param>
        /// <param name="type">The type of interaction.</param>
        /// <param name="name">The name of the ApplicationCommand.</param>
        /// <param name="user">The user who invoked the interaction.</param>
        [JsonConstructor]
        public MessageInteraction(Snowflake id, InteractionType type, string name, User user)
        {
            Id = id;
            Type = type;
            Name = name;
            User = user;
        }

        /// <summary>
        ///     Id of the interaction.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The type of interaction.
        /// </summary>
        [JsonPropertyName("type")]
        public InteractionType Type { get; }

        /// <summary>
        ///     The name of the ApplicationCommand.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     The user who invoked the interaction.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; }
    }
}
