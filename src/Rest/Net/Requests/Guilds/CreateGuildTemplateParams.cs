using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateGuildTemplateParams
    {
        /// <summary>
        /// Name of the template (1-100 characters).
        /// </summary>
        public string? Name { get; set; }  // Required property candidate

        /// <summary>
        /// Description for the template (0-120 characters).
        /// </summary>
        public Optional<string?> Description { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Name, nameof(Name));
            Preconditions.LengthAtLeast(Name, GuildTemplate.MinTemplateNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name, GuildTemplate.MaxTemplateNameLength, nameof(Name));
            Preconditions.LengthAtLeast(Description, GuildTemplate.MinTemplateDescriptionLength, nameof(Description));
            Preconditions.LengthAtMost(Description, GuildTemplate.MaxTemplateDescriptionLength, nameof(Description));
        }
    }
}
