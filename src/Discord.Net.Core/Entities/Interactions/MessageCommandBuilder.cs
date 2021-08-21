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

                // Discord updated the docs, this regex prevents special characters like @!$%(... etc,
                // https://discord.com/developers/docs/interactions/slash-commands#applicationcommand
                if (!Regex.IsMatch(value, @"^[\w -]{3,32}$"))
                    throw new ArgumentException("Command name cannot contain any special characters or whitespaces!");

                _name = value;
            }
        }

        private string _name { get; set; }

        /// <summary>
        ///     Build the current builder into a <see cref="ContextMenuCommandCreationProperties"/> class.
        /// </summary>
        /// <returns>A <see cref="ContextMenuCommandCreationProperties"/> that can be used to create message commands over rest.</returns>
        public ContextMenuCommandCreationProperties Build()
        {
            ContextMenuCommandCreationProperties props = new ContextMenuCommandCreationProperties()
            {
                Name = this.Name,
                Type=ApplicationCommandType.Message
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
