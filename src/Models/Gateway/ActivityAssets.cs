using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an activity assets object.
    /// </summary>
    public record ActivityAssets
    {
        /// <summary>
        ///     Creates a <see cref="ActivityAssets"/> with the provided parameters.
        /// </summary>
        /// <param name="largeImage">The id for a large asset of the activity, usually a snowflake.</param>
        /// <param name="largeText">Text displayed when hovering over the large image of the activity.</param>
        /// <param name="smallImage">The id for a small asset of the activity, usually a snowflake.</param>
        /// <param name="smallText">Text displayed when hovering over the small image of the activity.</param>
        [JsonConstructor]
        public ActivityAssets(Optional<string> largeImage, Optional<string> largeText, Optional<string> smallImage, Optional<string> smallText)
        {
            LargeImage = largeImage;
            LargeText = largeText;
            SmallImage = smallImage;
            SmallText = smallText;
        }

        /// <summary>
        ///     The id for a large asset of the activity, usually a snowflake.
        /// </summary>
        [JsonPropertyName("large_image")]
        public Optional<string> LargeImage { get; }

        /// <summary>
        ///     Text displayed when hovering over the large image of the activity.
        /// </summary>
        [JsonPropertyName("large_text")]
        public Optional<string> LargeText { get; }

        /// <summary>
        ///     The id for a small asset of the activity, usually a snowflake.
        /// </summary>
        [JsonPropertyName("small_image")]
        public Optional<string> SmallImage { get; }

        /// <summary>
        ///     Text displayed when hovering over the small image of the activity.
        /// </summary>
        [JsonPropertyName("small_text")]
        public Optional<string> SmallText { get; }
    }
}
