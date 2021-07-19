using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateGuildfromGuildTemplateParams
    {
        /// <summary>
        /// Name of the guild (2-100 characters).
        /// </summary>
        public string? Name { get; set; } // Required property candidate

        /// <summary>
        /// Image for the guild icon.
        /// </summary>
        public Optional<Image> Icon { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Name, nameof(Name));
            Preconditions.LengthAtLeast(Name, Guild.MinGuildNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name, Guild.MaxGuildNameLength, nameof(Name));
        }
    }
}
