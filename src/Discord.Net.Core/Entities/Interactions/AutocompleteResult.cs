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
                if (value == null)
                    throw new ArgumentNullException(nameof(value), $"{nameof(Name)} cannot be null.");
                _name = value.Length switch
                {
                    > 100 => throw new ArgumentOutOfRangeException(nameof(value), "Name length must be less than or equal to 100."),
                    0 => throw new ArgumentOutOfRangeException(nameof(value), "Name length must be at least 1."),
                    _ => value
                };
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
                    throw new ArgumentException($"{nameof(value)} must be a numeric type or a string!");

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
