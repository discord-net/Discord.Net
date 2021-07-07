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
    public class SlashCommandBuilder
    {
        /// <summary> 
        ///     Returns the maximun length a commands name allowed by Discord
        /// </summary>
        public const int MaxNameLength = 32;
        /// <summary> 
        ///     Returns the maximum length of a commands description allowed by Discord. 
        /// </summary>
        public const int MaxDescriptionLength = 100;
        /// <summary> 
        ///     Returns the maximum count of command options allowed by Discord
        /// </summary>
        public const int MaxOptionsCount = 10;

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
                if (!Regex.IsMatch(value, @"^[\w-]{3,32}$"))
                    throw new ArgumentException("Command name cannot contian any special characters or whitespaces!");

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
                Preconditions.AtLeast(value.Length, 1, nameof(Description));
                Preconditions.AtMost(value.Length, MaxDescriptionLength, nameof(Description));

                _description = value;
            }
        }

        /// <summary>
        ///     Gets or sets the options for this command.
        /// </summary>
        public List<SlashCommandOptionBuilder> Options
        {
            get => _options;
            set
            {
                if (value != null)
                    if (value.Count > MaxOptionsCount)
                        throw new ArgumentException(message: $"Option count must be less than or equal to {MaxOptionsCount}.", paramName: nameof(Options));

                _options = value;
            }
        }

        /// <summary>
        ///     Whether the command is enabled by default when the app is added to a guild
        /// </summary>
        public bool DefaultPermission { get; set; } = true;

        private string _name { get; set; }
        private string _description { get; set; }
        private List<SlashCommandOptionBuilder> _options { get; set; }

        /// <summary>
        ///     Build the current builder into a <see cref="SlashCommandCreationProperties"/> class.
        /// </summary>
        /// <returns>A <see cref="SlashCommandCreationProperties"/> that can be used to create slash commands over rest.</returns>
        public SlashCommandCreationProperties Build()
        {
            SlashCommandCreationProperties props = new SlashCommandCreationProperties()
            {
                Name = this.Name,
                Description = this.Description,
                DefaultPermission = this.DefaultPermission
            };

            if (this.Options != null && this.Options.Any())
            {
                var options = new List<ApplicationCommandOptionProperties>();

                this.Options.ForEach(x => options.Add(x.Build()));

                props.Options = options;
            }

            return props;

        }

        /// <summary>
        ///     Sets the field name.
        /// </summary>
        /// <param name="name">The value to set the field name to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SlashCommandBuilder WithName(string name)
        {
            this.Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the description of the current command.
        /// </summary>
        /// <param name="description">The description of this command.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithDescription(string description)
        {
            this.Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the default permission of the current command.
        /// </summary>
        /// <param name="value">The default permission value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithDefaultPermission(bool value)
        {
            this.DefaultPermission = value;
            return this;
        }

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="required">If this option is required for this command.</param>
        /// <param name="isDefault">If this option is the default option.</param>
        /// <param name="options">The options of the option to add.</param>
        /// <param name="choices">The choices of this option.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(string name, ApplicationCommandOptionType type,
           string description, bool required = true, bool isDefault = false, List<SlashCommandOptionBuilder> options = null, params ApplicationCommandOptionChoiceProperties[] choices)
        {
            // Make sure the name matches the requirements from discord
            Preconditions.NotNullOrEmpty(name, nameof(name));
            Preconditions.AtLeast(name.Length, 3, nameof(name));
            Preconditions.AtMost(name.Length, MaxNameLength, nameof(name));

            // Discord updated the docs, this regex prevents special characters like @!$%( and s p a c e s.. etc,
            // https://discord.com/developers/docs/interactions/slash-commands#applicationcommand
            if (!Regex.IsMatch(name, @"^[\w-]{3,32}$"))
                throw new ArgumentException("Command name cannot contian any special characters or whitespaces!", nameof(name));

            // same with description
            Preconditions.NotNullOrEmpty(description, nameof(description));
            Preconditions.AtLeast(description.Length, 3, nameof(description));
            Preconditions.AtMost(description.Length, MaxDescriptionLength, nameof(description));

            // make sure theres only one option with default set to true
            if (isDefault)
            {
                if (this.Options != null)
                    if (this.Options.Any(x => x.Default.HasValue && x.Default.Value))
                        throw new ArgumentException("There can only be one command option with default set to true!", nameof(isDefault));
            }

            SlashCommandOptionBuilder option = new SlashCommandOptionBuilder();
            option.Name = name;
            option.Description = description;
            option.Required = required;
            option.Default = isDefault;
            option.Options = options;
            option.Type = type;
            option.Choices = choices != null ? new List<ApplicationCommandOptionChoiceProperties>(choices) : null;

            return AddOption(option);
        }

        ///// <summary>
        /////     Adds an option to the current slash command.
        ///// </summary>
        ///// <param name="name">The name of the option to add.</param>
        ///// <param name="type">The type of this option.</param>
        ///// <param name="description">The description of this option.</param>
        ///// <param name="required">If this option is required for this command.</param>
        ///// <param name="isDefault">If this option is the default option.</param>
        ///// <param name="choices">The choices of this option.</param>
        ///// <returns>The current builder.</returns>
        //public SlashCommandBuilder AddOption(string name, ApplicationCommandOptionType type,
        //    string description, bool required = true, bool isDefault = false, params ApplicationCommandOptionChoiceProperties[] choices)
        //    => AddOption(name, type, description, required, isDefault, null, choices);

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The sescription of this option.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(string name, ApplicationCommandOptionType type, string description)
            => AddOption(name, type, description, options: null, choices: null);

        /// <summary>
        ///     Adds an option to this slash command.
        /// </summary>
        /// <param name="option">The option to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(SlashCommandOptionBuilder option)
        {
            if (this.Options == null)
                this.Options = new List<SlashCommandOptionBuilder>();

            if (this.Options.Count >= MaxOptionsCount)
                throw new ArgumentOutOfRangeException(nameof(Options), $"Cannot have more than {MaxOptionsCount} options!");

            if (option == null)
                throw new ArgumentNullException(nameof(option), "Option cannot be null");

            this.Options.Add(option);
            return this;
        }
        /// <summary>
        ///     Adds a collection of options to the current slash command.
        /// </summary>
        /// <param name="options">The collection of options to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOptions(params SlashCommandOptionBuilder[] options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null!");

            if (options.Length == 0)
                throw new ArgumentException(nameof(options), "Options cannot be empty!");

            if (this.Options == null)
                this.Options = new List<SlashCommandOptionBuilder>();

            if (this.Options.Count + options.Length > MaxOptionsCount)
                throw new ArgumentOutOfRangeException(nameof(options), $"Cannot have more than {MaxOptionsCount} options!");

            this.Options.AddRange(options);
            return this;
        }
    }

    /// <summary>
    ///     Represents a class used to build options for the <see cref="SlashCommandBuilder"/>.
    /// </summary>
    public class SlashCommandOptionBuilder
    {
        /// <summary>
        ///     The max length of a choice's name allowed by Discord.
        /// </summary>
        public const int ChoiceNameMaxLength = 100;

        /// <summary>
        ///     The maximum number of choices allowed by Discord.
        /// </summary>
        public const int MaxChoiceCount = 10;

        private string _name;
        private string _description;

        /// <summary>
        ///     The name of this option.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value?.Length > SlashCommandBuilder.MaxNameLength)
                    throw new ArgumentException("Name length must be less than or equal to 32");
                if (value?.Length < 1)
                    throw new ArgumentException("Name length must at least 1 characters in length");

                if (value != null)
                    if (!Regex.IsMatch(value, @"^[\w-]{3,32}$"))
                        throw new ArgumentException("Option name cannot contian any special characters or whitespaces!");

                _name = value;
            }
        }

        /// <summary>
        ///     The description of this option.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (value?.Length > SlashCommandBuilder.MaxDescriptionLength)
                    throw new ArgumentException("Description length must be less than or equal to 100");
                if (value?.Length < 1)
                    throw new ArgumentException("Name length must at least 1 character in length");

                _description = value;
            }
        }

        /// <summary>
        ///     The type of this option.
        /// </summary>
        public ApplicationCommandOptionType Type { get; set; }

        /// <summary>
        ///     The first required option for the user to complete. only one option can be default.
        /// </summary>
        public bool? Default { get; set; }

        /// <summary>
        ///     <see langword="true"/> if this option is required for this command, otherwise <see langword="false"/>.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     choices for string and int types for the user to pick from.
        /// </summary>
        public List<ApplicationCommandOptionChoiceProperties> Choices { get; set; }

        /// <summary>
        ///     If the option is a subcommand or subcommand group type, this nested options will be the parameters.
        /// </summary>
        public List<SlashCommandOptionBuilder> Options { get; set; }

        /// <summary>
        ///     Builds the current option.
        /// </summary>
        /// <returns>The built version of this option.</returns>
        public ApplicationCommandOptionProperties Build()
        {
            bool isSubType = this.Type == ApplicationCommandOptionType.SubCommandGroup;

            if (isSubType && (Options == null || !Options.Any()))
                throw new ArgumentException(nameof(Options), "SubCommands/SubCommandGroups must have at least one option");

            if (!isSubType && (Options != null && Options.Any()) && Type != ApplicationCommandOptionType.SubCommand)
                throw new ArgumentException(nameof(Options), $"Cannot have options on {Type} type");

            return new ApplicationCommandOptionProperties()
            {
                Name = this.Name,
                Description = this.Description,
                Default = this.Default,
                Required = this.Required,
                Type = this.Type,
                Options = this.Options?.Count > 0 ? new List<ApplicationCommandOptionProperties>(this.Options.Select(x => x.Build())) : null,
                Choices = this.Choices
            };
        }

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="required">If this option is required for this command.</param>
        /// <param name="isDefault">If this option is the default option.</param>
        /// <param name="options">The options of the option to add.</param>
        /// <param name="choices">The choices of this option.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddOption(string name, ApplicationCommandOptionType type,
           string description, bool required = true, bool isDefault = false, List<SlashCommandOptionBuilder> options = null, params ApplicationCommandOptionChoiceProperties[] choices)
        {
            // Make sure the name matches the requirements from discord
            Preconditions.NotNullOrEmpty(name, nameof(name));
            Preconditions.AtLeast(name.Length, 3, nameof(name));
            Preconditions.AtMost(name.Length, SlashCommandBuilder.MaxNameLength, nameof(name));

            // Discord updated the docs, this regex prevents special characters like @!$%( and s p a c e s.. etc,
            // https://discord.com/developers/docs/interactions/slash-commands#applicationcommand
            if (!Regex.IsMatch(name, @"^[\w-]{3,32}$"))
                throw new ArgumentException("Command name cannot contian any special characters or whitespaces!", nameof(name));

            // same with description
            Preconditions.NotNullOrEmpty(description, nameof(description));
            Preconditions.AtLeast(description.Length, 3, nameof(description));
            Preconditions.AtMost(description.Length, SlashCommandBuilder.MaxDescriptionLength, nameof(description));

            // make sure theres only one option with default set to true
            if (isDefault)
            {
                if (this.Options != null)
                    if (this.Options.Any(x => x.Default.HasValue && x.Default.Value))
                        throw new ArgumentException("There can only be one command option with default set to true!", nameof(isDefault));
            }

            SlashCommandOptionBuilder option = new SlashCommandOptionBuilder();
            option.Name = name;
            option.Description = description;
            option.Required = required;
            option.Default = isDefault;
            option.Options = options;
            option.Type = type;
            option.Choices = choices != null ? new List<ApplicationCommandOptionChoiceProperties>(choices) : null;

            return AddOption(option);
        }
        /// <summary>
        ///     Adds a sub option to the current option.
        /// </summary>
        /// <param name="option">The sub option to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddOption(SlashCommandOptionBuilder option)
        {
            if (this.Options == null)
                this.Options = new List<SlashCommandOptionBuilder>();

            if (this.Options.Count >= SlashCommandBuilder.MaxOptionsCount)
                throw new ArgumentOutOfRangeException(nameof(Choices), $"There can only be {SlashCommandBuilder.MaxOptionsCount} options per sub command group!");

            if (option == null)
                throw new ArgumentNullException(nameof(option), "Option cannot be null");

            Options.Add(option);
            return this;
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, int value)
        {
            if (Choices == null)
                Choices = new List<ApplicationCommandOptionChoiceProperties>();

            if (Choices.Count >= MaxChoiceCount)
                throw new ArgumentOutOfRangeException(nameof(Choices), $"Cannot add more than {MaxChoiceCount} choices!");

            if (name == null)
                throw new ArgumentNullException($"{nameof(name)} cannot be null!");

            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, 100, nameof(name));

            Choices.Add(new ApplicationCommandOptionChoiceProperties()
            {
                Name = name,
                Value = value
            });

            return this;
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, string value)
        {
            if (Choices == null)
                Choices = new List<ApplicationCommandOptionChoiceProperties>();

            if (Choices.Count >= MaxChoiceCount)
                throw new ArgumentOutOfRangeException(nameof(Choices), $"Cannot add more than {MaxChoiceCount} choices!");

            if (name == null)
                throw new ArgumentNullException($"{nameof(name)} cannot be null!");

            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null!");

            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, 100, nameof(name));

            Preconditions.AtLeast(value.Length, 1, nameof(value));
            Preconditions.AtMost(value.Length, 100, nameof(value));

            Choices.Add(new ApplicationCommandOptionChoiceProperties()
            {
                Name = name,
                Value = value
            });

            return this;
        }

        /// <summary>
        ///     Sets the current builders name.
        /// </summary>
        /// <param name="name">The name to set the current option builder.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithName(string name)
        {
            this.Name = name;

            return this;
        }

        /// <summary>
        ///     Sets the current builders description.
        /// </summary>
        /// <param name="description">The description to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithDescription(string description)
        {
            this.Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the current builders required field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithRequired(bool value)
        {
            this.Required = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders default field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithDefault(bool value)
        {
            this.Default = value;
            return this;
        }

        /// <summary>
        ///     Sets the current type of this builder.
        /// </summary>
        /// <param name="type">The type to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithType(ApplicationCommandOptionType type)
        {
            this.Type = type;
            return this;
        }
    }
}
