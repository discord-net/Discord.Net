using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a channel mention object.
    /// </summary>
    public record ChannelMention
    {
        /// <summary>
        ///     Creates a <see cref="ChannelMention"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Id of the channel.</param>
        /// <param name="guildId">Id of the guild containing the channel.</param>
        /// <param name="type">The type of channel.</param>
        /// <param name="name">The name of the channel.</param>
        [JsonConstructor]
        public ChannelMention(Snowflake id, Snowflake guildId, ChannelType type, string name)
        {
            Id = id;
            GuildId = guildId;
            Type = type;
            Name = name;
        }

        /// <summary>
        ///     Id of the channel.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Id of the guild containing the channel.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; }

        /// <summary>
        ///     The type of channel.
        /// </summary>
        [JsonPropertyName("type")]
        public ChannelType Type { get; }

        /// <summary>
        ///     The name of the channel.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }
    }
}
