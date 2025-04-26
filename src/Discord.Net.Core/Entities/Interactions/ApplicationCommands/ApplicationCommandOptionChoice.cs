using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Discord
{
    /// <summary>
    ///     Represents a choice for a <see cref="IApplicationCommandInteractionDataOption"/>. This class is used when making new commands.
    /// </summary>
    public class ApplicationCommandOptionChoiceProperties
    {
        private string _name;
        private object _value;
        private IDictionary<string, string> _nameLocalizations = new Dictionary<string, string>();

        /// <summary>
        ///     Gets or sets the name of this choice.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value is not null)
                {
                    Preconditions.AtLeast(value.Length, 1, nameof(Name));
                    Preconditions.AtMost(value.Length, 100, nameof(Name));
                }

                _name = value;
            }
        }

        /// <summary>
        ///     Gets the value of this choice.
        ///     <note type="warning">
        ///         Discord only accepts int, double/floats, and string as the input.
        ///     </note>
        /// </summary>
        public object Value
        {
            get => _value;
            set
            {
                if (value is not null && value is not string && !value.IsNumericType())
                    throw new ArgumentException($"The value of a choice must be a string or a numeric type! Value: \"{value}\"");
                _value = value;
            }
        }

        /// <summary>
        ///     Gets or sets the localization dictionary for the name field of this choice.
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
                            throw new ArgumentException($"Key values of the dictionary must be valid language codes. Locale: \"{locale}\"");

                        Preconditions.AtLeast(name.Length, 1, nameof(name), msg: $"Name value of locale {locale} cannot be empty.");
                        Preconditions.AtMost(name.Length, 100, nameof(name), msg: $"Name value of locale {locale} have to contains 100 chars at most.");
                    }
                }

                _nameLocalizations = value;
            }
        }
    }
}
