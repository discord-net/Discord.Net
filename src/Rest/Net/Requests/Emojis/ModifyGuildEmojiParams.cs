using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyGuildEmojiParams
    {
        /// <summary>
        /// Name of the <see cref="Emoji"/>.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        /// <see cref="Role"/>s allowed to use this <see cref="Emoji"/>.
        /// </summary>
        public Optional<Snowflake[]?> Roles { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrWhitespace(Name!, nameof(Name));
        }
    }
}
