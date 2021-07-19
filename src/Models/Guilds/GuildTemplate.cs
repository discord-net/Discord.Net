using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord guild template object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild-template#guild-template-object-guild-template-structure"/>
    /// </remarks>
    public record GuildTemplate
    {
        /// <summary>
        /// Minimum langth of a template name.
        /// </summary>
        public const int MinTemplateNameLength = 1;

        /// <summary>
        /// Maximum langth of a template name.
        /// </summary>
        public const int MaxTemplateNameLength = 100;

        /// <summary>
        /// Minimum langth of a template description.
        /// </summary>
        public const int MinTemplateDescriptionLength = 0;

        /// <summary>
        /// Maximum langth of a template description.
        /// </summary>
        public const int MaxTemplateDescriptionLength = 120;

        /// <summary>
        /// The template code (unique ID).
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; init; } // Required property candidate

        /// <summary>
        /// Template name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// The description for the template.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; }

        /// <summary>
        /// Number of times this template has been used.
        /// </summary>
        [JsonPropertyName("usage_count")]
        public int UsageCount { get; init; }

        /// <summary>
        /// The ID of the user who created the template.
        /// </summary>
        [JsonPropertyName("creator_id")]
        public Snowflake CreatorId { get; init; }

        /// <summary>
        /// The user who created the template.
        /// </summary>
        [JsonPropertyName("creator")]
        public User? Creator { get; init; } // Required property candidate

        /// <summary>
        /// When this template was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }

        /// <summary>
        /// When this template was last synced to the source guild.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; init; }

        /// <summary>
        /// The ID of the <see cref="Guild"/> this template is based on.
        /// </summary>
        [JsonPropertyName("source_guild_id")]
        public Snowflake SourceGuildId { get; init; }

        /// <summary>
        /// The <see cref="Guild"/> snapshot this template contains.
        /// </summary>
        [JsonPropertyName("serialized_source_guild")]
        public Guild? SerializedSourceGuild { get; init; } // Required property candidate

        /// <summary>
        /// Whether the template has unsynced changes.
        /// </summary>
        [JsonPropertyName("is_dirty")]
        public bool? IsDirty { get; init; }
    }
}
