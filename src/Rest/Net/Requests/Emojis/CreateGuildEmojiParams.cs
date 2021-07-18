using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateGuildEmojiParams
    {
        /// <summary>
        /// Name of the <see cref="Emoji"/>.
        /// </summary>
        public string? Name { get; set; } // Required property candidate

        /// <summary>
        /// The 128x128 <see cref="Emoji"/> image.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// <see cref="Role"/>s allowed to use this <see cref="Emoji"/>.
        /// </summary>
        public Snowflake[]? Roles { get; set; } // Required property candidate

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrWhitespace(Name, nameof(Name));
        }
    }
}
