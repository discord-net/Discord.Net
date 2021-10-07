using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            set
            {
                if(value?.Length > 100)
                    throw new ArgumentException("Name length must be less than or equal to 100");
                if (value?.Length < 1)
                    throw new ArgumentException("Name length must at least 1 character in length");
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of this choice.
        ///     <note type="warning">
        ///         Discord only accepts int and string as the input.
        ///     </note>
        /// </summary>
        public object Value
        {
            get => _value;
            set
            {
                if(value != null)
                {
                    if(!(value is int) && !(value is string))
                        throw new ArgumentException("The value of a choice must be a string or int!");
                }
                _value = value;
            }
        }
    }
}
