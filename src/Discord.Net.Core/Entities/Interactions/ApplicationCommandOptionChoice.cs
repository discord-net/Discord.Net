using System;

namespace Discord
{
    /// <summary>
    ///     Represents a choice for a <see cref="IApplicationCommandInteractionDataOption"/>. This class is used when making new commands.
    /// </summary>
    public class ApplicationCommandOptionChoiceProperties
    {
        private string _name;
        private object _value;
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
        ///     Gets or sets the value of this choice.
        ///     <note type="warning">
        ///         Discord only accepts int, string, and doubles as the input.
        ///     </note>
        /// </summary>
        public object Value
        {
            get => _value;
            set
            {
                if (value != null && value is not int && value is not string && value is not double)
                    throw new ArgumentException("The value of a choice must be a string, int, or double!");
                _value = value;
            }
        }
    }
}
