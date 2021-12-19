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

        /// <summary>
        ///     Gets or sets the name of this option.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), $"{nameof(Name)} cannot be null.");

                if (value.Length > 32)
                    throw new ArgumentOutOfRangeException(nameof(value), "Name length must be less than or equal to 32.");

                if (!Regex.IsMatch(value, @"^[\w-]{1,32}$"))
                    throw new FormatException($"{nameof(value)} must match the regex ^[\\w-]{{1,32}}$");

                if (value.Any(x => char.IsUpper(x)))
                    throw new FormatException("Name cannot contain any uppercase characters.");

                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the description of this option.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value?.Length switch
            {
                > 100 => throw new ArgumentOutOfRangeException(nameof(value), "Description length must be less than or equal to 100."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Description length must be at least 1."),
                _ => value
            };
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
    }
}
