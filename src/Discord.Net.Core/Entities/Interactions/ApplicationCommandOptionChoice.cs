using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            set => _name = value?.Length switch
            {
                > 100 => throw new ArgumentOutOfRangeException(nameof(value), "Name length must be less than or equal to 100."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Name length must at least 1."),
                _ => value
            };
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
                if (value != null && value is not string && !value.IsNumericType())
                    throw new ArgumentException("The value of a choice must be a string or a numeric type!");
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
                            throw new ArgumentException("Key values of the dictionary must be valid language codes.");

                        switch (name.Length)
                        {
                            case > 100:
                                throw new ArgumentOutOfRangeException(nameof(value),
                                    "Name length must be less than or equal to 100.");
                            case 0:
                                throw new ArgumentOutOfRangeException(nameof(value), "Name length must at least 1.");
                        }
                    }
                }

                _nameLocalizations = value;
            }
        }
    }
}
