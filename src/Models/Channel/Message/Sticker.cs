using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a sticker object.
    /// </summary>
    public record Sticker
    {
        /// <summary>
        ///     Creates a <see cref="Sticker"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Id of the sticker.</param>
        /// <param name="packId">Id of the pack the sticker is from.</param>
        /// <param name="name">Name of the sticker.</param>
        /// <param name="description">Description of the sticker.</param>
        /// <param name="tags">A comma-separated list of tags for the sticker.</param>
        /// <param name="asset">Sticker asset hash.</param>
        /// <param name="formatType">Type of sticker format.</param>
        [JsonConstructor]
        public Sticker(Snowflake id, Snowflake packId, string name, string description, Optional<string> tags, string asset, StickerFormatType formatType)
        {
            Id = id;
            PackId = packId;
            Name = name;
            Description = description;
            Tags = tags;
            Asset = asset;
            FormatType = formatType;
        }

        /// <summary>
        ///     Id of the sticker.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Id of the pack the sticker is from.
        /// </summary>
        [JsonPropertyName("pack_id")]
        public Snowflake PackId { get; }

        /// <summary>
        ///     Name of the sticker.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     Description of the sticker.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; }

        /// <summary>
        ///     A comma-separated list of tags for the sticker.
        /// </summary>
        [JsonPropertyName("tags")]
        public Optional<string> Tags { get; }

        /// <summary>
        ///     Sticker asset hash.
        /// </summary>
        [JsonPropertyName("asset")]
        public string Asset { get; }

        /// <summary>
        ///     Type of sticker format.
        /// </summary>
        [JsonPropertyName("format_type")]
        public StickerFormatType FormatType { get; }
    }
}
