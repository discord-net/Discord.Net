using System;

namespace Discord
{
    /// <summary>
    ///     Represents a result to an autocomplete interaction.
    /// </summary>
    public class AutocompleteResult
    {
        private object _value;
        private string _name;

        /// <summary>
        ///     Gets or sets the name of the result.
        /// </summary>
        /// <remarks>
        ///     Name cannot be null and has to be between 1-100 characters in length.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public string Name
        {
            get => _name;
            set
            {
                Preconditions.NotNull(value, nameof(Name));
                Preconditions.AtLeast(value.Length, 1, nameof(Name));
                Preconditions.AtMost(value.Length, 100, nameof(Name));
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the result.
        /// </summary>
        /// <remarks>
        ///     Only <see cref="string"/>, <see cref="int"/>, and <see cref="double"/> are allowed for a value.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public object Value
        {
            get => _value;
            set
            {
                if (value is not string && !value.IsNumericType())
                    throw new ArgumentException($"{nameof(value)} must be a numeric type or a string! Value: \"{value}\"");

                _value = value;
            }
        }

        /// <summary>
        ///     Creates a new <see cref="AutocompleteResult"/>.
        /// </summary>
        public AutocompleteResult() { }

        /// <summary>
        ///     Creates a new <see cref="AutocompleteResult"/> with the passed in <paramref name="name"/> and <paramref name="value"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public AutocompleteResult(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
