using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                    throw new ArgumentNullException($"{nameof(Name)} cannot be null!");

                if (value.Length > 32)
                    throw new ArgumentException($"{nameof(Name)} length must be less than or equal to 32");

                if (!Regex.IsMatch(value, @"^[\w-]{1,32}$"))
                    throw new ArgumentException($"{nameof(Name)} must match the regex ^[\\w-]{{1,32}}$");

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
                if (value?.Length > 100)
                    throw new ArgumentException("Description length must be less than or equal to 100");
                if (value?.Length < 1)
                    throw new ArgumentException("Description length must at least 1 character in length");
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
        public bool? Default { get; set; }

        /// <summary>
        ///     Gets or sets if the option is required.
        /// </summary>
        public bool? Required { get; set; }

        /// <summary>
        ///     Gets or sets whether or not this option supports autocomplete.
        /// </summary>
        public bool Autocomplete { get; set; }
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
