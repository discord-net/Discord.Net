using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord role tags object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/permissions#role-object-role-tags-structure"/>
    /// </remarks>
    public record RoleTags
    {
        /// <summary>
        /// The id of the bot this <see cref="Role"/> belongs to.
        /// </summary>
        [JsonPropertyName("bot_id")]
        public Optional<Snowflake> BotId { get; }

        /// <summary>
        /// The id of the <see cref="Integration"/> this role belongs to.
        /// </summary>
        [JsonPropertyName("integration_id")]
        public Optional<Snowflake> IntegrationId { get; }

        /// <summary>
        /// Whether this is the <see cref="Guild"/>'s premium subscriber <see cref="Role"/>.
        /// </summary>
        [JsonPropertyName("premium_subscriber")]
        public Optional<bool?> PremiumSubscriber { get; }
    }
}
