using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a result to an autocomplete interaction.
    /// </summary>
    public class AutocompleteResult
    {
        private object _value { get; set; }
        private string _name { get; set; }

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
                    throw new ArgumentException("Name cannot be null!");
                if (value.Length > 100)
                    throw new ArgumentException("Name length must be less than or equal to 100 characters in length!");
                if (value.Length < 1)
                    throw new ArgumentException("Name length must at least 1 character in length!");
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
                if (value == null)
                    throw new ArgumentNullException("Value cannot be null");

                _value = value switch
                {
                    string str => str,
                    int integer => integer,
                    long lng => lng,
                    double number => number,
                    _ => throw new ArgumentException($"Type {value.GetType().Name} cannot be set as a value! Only string, int, and double allowed!"),
                };
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
