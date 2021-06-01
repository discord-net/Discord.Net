using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a guild template object.
    /// </summary>
    public record GuildTemplate
    {
        /// <summary>
        ///     Creates a <see cref="GuildTemplate"/> with the provided parameters.
        /// </summary>
        /// <param name="code">The template code (unique ID).</param>
        /// <param name="name">Template name.</param>
        /// <param name="description">The description for the template.</param>
        /// <param name="usageCount">Number of times this template has been used.</param>
        /// <param name="creatorId">The ID of the user who created the template.</param>
        /// <param name="creator">The user who created the template.</param>
        /// <param name="createdAt">When this template was created.</param>
        /// <param name="updatedAt">When this template was last synced to the source guild.</param>
        /// <param name="sourceGuildId">The ID of the guild this template is based on.</param>
        /// <param name="serializedSourceGuild">The guild snapshot this template contains.</param>
        /// <param name="isDirty">Whether the template has unsynced changes.</param>
        [JsonConstructor]
        public GuildTemplate(string code, string name, string? description, int usageCount, Snowflake creatorId, User creator, DateTimeOffset createdAt, DateTimeOffset updatedAt, Snowflake sourceGuildId, Guild serializedSourceGuild, bool? isDirty)
        {
            Code = code;
            Name = name;
            Description = description;
            UsageCount = usageCount;
            CreatorId = creatorId;
            Creator = creator;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            SourceGuildId = sourceGuildId;
            SerializedSourceGuild = serializedSourceGuild;
            IsDirty = isDirty;
        }

        /// <summary>
        ///     The template code (unique ID).
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; }

        /// <summary>
        ///     Template name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     The description for the template.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; }

        /// <summary>
        ///     Number of times this template has been used.
        /// </summary>
        [JsonPropertyName("usage_count")]
        public int UsageCount { get; }

        /// <summary>
        ///     The ID of the user who created the template.
        /// </summary>
        [JsonPropertyName("creator_id")]
        public Snowflake CreatorId { get; }

        /// <summary>
        ///     The user who created the template.
        /// </summary>
        [JsonPropertyName("creator")]
        public User Creator { get; }

        /// <summary>
        ///     When this template was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; }

        /// <summary>
        ///     When this template was last synced to the source guild.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; }

        /// <summary>
        ///     The ID of the guild this template is based on.
        /// </summary>
        [JsonPropertyName("source_guild_id")]
        public Snowflake SourceGuildId { get; }

        /// <summary>
        ///     The guild snapshot this template contains.
        /// </summary>
        [JsonPropertyName("serialized_source_guild")]
        public Guild SerializedSourceGuild { get; }

        /// <summary>
        ///     Whether the template has unsynced changes.
        /// </summary>
        [JsonPropertyName("is_dirty")]
        public bool? IsDirty { get; }
    }
}
