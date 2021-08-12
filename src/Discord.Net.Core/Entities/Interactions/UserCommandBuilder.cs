using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     A class used to build slash commands.
    /// </summary>
    public class UserCommandBuilder
    {
        /// <summary> 
        ///     Returns the maximun length a commands name allowed by Discord
        /// </summary>
        public const int MaxNameLength = 32;
        /// <summary> 
        ///     Returns the maximum length of a commands description allowed by Discord. 
        /// </summary>
        public const int MaxDescriptionLength = 0;

        /// <summary>
        ///     The name of this slash command.
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

        /// <summary>
        ///    A 1-100 length description of this slash command
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                Preconditions.Equals(value, "");

                _description = value;
            }
        }

        private string _name { get; set; }
        private string _description { get; set; }

        /// <summary>
        ///     Build the current builder into a <see cref="UserCommandCreationProperties"/> class.
        /// </summary>
        /// <returns>A <see cref="UserCommandCreationProperties"/> that can be used to create user commands over rest.</returns>
        public UserCommandCreationProperties Build()
        {
            UserCommandCreationProperties props = new UserCommandCreationProperties()
            {
                Name = this.Name,
                Description = this.Description,
                Type=ApplicationCommandType.User
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
        public UserCommandBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the description of the current command.
        /// </summary>
        /// <param name="description">The description of this command.</param>
        /// <returns>The current builder.</returns>
        public UserCommandBuilder WithDescription(string description)
        {
            this.Description = description;
            return this;
        }        
    }
}
