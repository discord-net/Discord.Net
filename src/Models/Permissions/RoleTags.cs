using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a role tags object.
    /// </summary>
    public record RoleTags
    {
        /// <summary>
        ///     Creates a <see cref="RoleTags"/> with the provided parameters.
        /// </summary>
        /// <param name="botId">The id of the bot this role belongs to.</param>
        /// <param name="integrationId">The id of the integration this role belongs to.</param>
        /// <param name="premiumSubscriber">Whether this is the guild's premium subscriber role.</param>
        [JsonConstructor]
        public RoleTags(Optional<Snowflake> botId, Optional<Snowflake> integrationId, Optional<bool?> premiumSubscriber)
        {
            BotId = botId;
            IntegrationId = integrationId;
            PremiumSubscriber = premiumSubscriber;
        }

        /// <summary>
        ///     The id of the bot this role belongs to.
        /// </summary>
        [JsonPropertyName("bot_id")]
        public Optional<Snowflake> BotId { get; }

        /// <summary>
        ///     The id of the integration this role belongs to.
        /// </summary>
        [JsonPropertyName("integration_id")]
        public Optional<Snowflake> IntegrationId { get; }

        /// <summary>
        ///     Whether this is the guild's premium subscriber role.
        /// </summary>
        [JsonPropertyName("premium_subscriber")]
        public Optional<bool?> PremiumSubscriber { get; }
    }
}
