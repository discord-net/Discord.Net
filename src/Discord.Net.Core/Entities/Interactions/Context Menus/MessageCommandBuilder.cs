using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     A class used to build Message commands.
    /// </summary>
    public class MessageCommandBuilder
    {
        /// <summary> 
        ///     Returns the maximun length a commands name allowed by Discord
        /// </summary>
        public const int MaxNameLength = 32;

        /// <summary>
        ///     The name of this Message command.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Preconditions.NotNullOrEmpty(value, nameof(Name));
                Preconditions.AtLeast(value.Length, 3, nameof(Name));
                Preconditions.AtMost(value.Length, MaxNameLength, nameof(Name));

                _name = value;
            }
        }

        private string _name { get; set; }

        /// <summary>
        ///     Build the current builder into a <see cref="MessageCommandProperties"/> class.
        /// </summary>
        /// <returns>
        ///     A <see cref="MessageCommandProperties"/> that can be used to create message commands.
        /// </returns>
        public MessageCommandProperties Build()
        {
            MessageCommandProperties props = new MessageCommandProperties()
            {
                Name = this.Name,
            };

            return props;

        }

        /// <summary>
        ///     Sets the field name.
        /// </summary>
        /// <param name="name">The value to set the field name to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public MessageCommandBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }      
    }
}
