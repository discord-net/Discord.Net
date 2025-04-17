using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="IApplicationCommandOption"/> for making slash commands.
    /// </summary>
    public class ApplicationCommandOptionProperties
    {
        private string _name;
        private string _description;
        private IDictionary<string, string> _nameLocalizations = new Dictionary<string, string>();
        private IDictionary<string, string> _descriptionLocalizations = new Dictionary<string, string>();

        /// <summary>
        ///     Gets or sets the name of this option.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                EnsureValidOptionName(value);
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the description of this option.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                EnsureValidOptionDescription(value);
                _description = value;
            }
        }

        /// <summary>
        ///     Gets or sets the type of this option.
        /// </summary>
        public ApplicationCommandOptionType Type { get; set; }

        /// <summary>
        ///     Gets or sets whether or not this options is the first required option for the user to complete. only one option can be default.
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        ///     Gets or sets if the option is required.
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        ///     Gets or sets whether or not this option supports autocomplete.
        /// </summary>
        public bool IsAutocomplete { get; set; }

        /// <summary>
        ///     Gets or sets the smallest number value the user can input.
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        ///     Gets or sets the largest number value the user can input.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the minimum allowed length for a string input.
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        ///     Gets or sets the maximum allowed length for a string input.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        ///     Gets or sets the choices for string and int types for the user to pick from.
        /// </summary>
        public List<ApplicationCommandOptionChoiceProperties> Choices { get; set; }

        /// <summary>
        ///     Gets or sets if this option is a subcommand or subcommand group type, these nested options will be the parameters.
        /// </summary>
        public List<ApplicationCommandOptionProperties> Options { get; set; }

        /// <summary>
        ///     Gets or sets the allowed channel types for this option.
        /// </summary>
        public List<ChannelType> ChannelTypes { get; set; }

        /// <summary>
        ///     Gets or sets the localization dictionary for the name field of this option.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when any of the dictionary keys is an invalid locale.</exception>
        public IDictionary<string, string> NameLocalizations
        {
            get => _nameLocalizations;
            set
            {
                if (value != null)
                {
                    foreach (var (locale, name) in value)
                    {
                        if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                            throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                        EnsureValidOptionName(name);
                    }
                }

                _nameLocalizations = value;
            }
        }

        /// <summary>
        ///     Gets or sets the localization dictionary for the description field of this option.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when any of the dictionary keys is an invalid locale.</exception>
        public IDictionary<string, string> DescriptionLocalizations
        {
            get => _descriptionLocalizations;
            set
            {
                if (value != null)
                {
                    foreach (var (locale, description) in value)
                    {
                        if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                            throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                        EnsureValidOptionDescription(description);
                    }
                }

                _descriptionLocalizations = value;
            }
        }

        private static void EnsureValidOptionName(string name)
        {
            Preconditions.NotNull(name, nameof(Name));
            Preconditions.AtMost(name.Length, 32, nameof(Name));

            if (!Regex.IsMatch(name, @"^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$"))
                throw new ArgumentException(@$"Name must match the regex ^[-_\p{{L}}\p{{N}}\p{{IsDevanagari}}\p{{IsThai}}]{{1,32}}$. Value: ""{name}""", nameof(name));

            if (name.Any(char.IsUpper))
                throw new FormatException($"Name cannot contain any uppercase characters. Value: \"{name}\"");
        }

        private static void EnsureValidOptionDescription(string description)
        {
            Preconditions.AtLeast(description.Length, 1, nameof(Description));
            Preconditions.AtMost(description.Length, 100, nameof(Description));
        }
    }
}
