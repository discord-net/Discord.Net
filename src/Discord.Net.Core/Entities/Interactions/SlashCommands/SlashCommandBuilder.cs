using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Discord
{
    /// <summary>
    ///     Represents a class used to build slash commands.
    /// </summary>
    public class SlashCommandBuilder
    {
        /// <summary>
        ///     Returns the maximum length a commands name allowed by Discord
        /// </summary>
        public const int MaxNameLength = 32;
        /// <summary>
        ///     Returns the maximum length of a commands description allowed by Discord.
        /// </summary>
        public const int MaxDescriptionLength = 100;
        /// <summary>
        ///     Returns the maximum count of command options allowed by Discord
        /// </summary>
        public const int MaxOptionsCount = 25;

        /// <summary>
        ///     Gets or sets the name of this slash command.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                EnsureValidCommandName(value);
                _name = value;
            }
        }

        /// <summary>
        ///    Gets or sets a 1-100 length description of this slash command
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                EnsureValidCommandDescription(value);
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
                Preconditions.AtMost(value?.Count ?? 0, MaxOptionsCount, nameof(value));
                _options = value;
            }
        }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations => _nameLocalizations;

        /// <summary>
        ///     Gets the localization dictionary for the description field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> DescriptionLocalizations => _descriptionLocalizations;

        /// <summary>
        ///     Gets the context types this command can be executed in. <see langword="null"/> if not set.
        /// </summary>
        public HashSet<InteractionContextType> ContextTypes { get; set; }

        /// <summary>
        ///     Gets the installation method for this command. <see langword="null"/> if not set.
        /// </summary>
        public HashSet<ApplicationIntegrationType> IntegrationTypes { get; set; }

        /// <summary>
        ///     Gets or sets whether the command is enabled by default when the app is added to a guild
        /// </summary>
        public bool IsDefaultPermission { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether or not this command can be used in DMs.
        /// </summary>
        [Obsolete("This property will be deprecated soon. Configure with ContextTypes instead.")]
        public bool IsDMEnabled { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether or not this command is age restricted.
        /// </summary>
        public bool IsNsfw { get; set; } = false;

        /// <summary>
        ///     Gets or sets the default permission required to use this slash command.
        /// </summary>
        public GuildPermission? DefaultMemberPermissions { get; set; }

        private string _name;
        private string _description;
        private Dictionary<string, string> _nameLocalizations;
        private Dictionary<string, string> _descriptionLocalizations;
        private List<SlashCommandOptionBuilder> _options;

        /// <summary>
        ///     Build the current builder into a <see cref="SlashCommandProperties"/> class.
        /// </summary>
        /// <returns>A <see cref="SlashCommandProperties"/> that can be used to create slash commands.</returns>
        public SlashCommandProperties Build()
        {
            // doing ?? 1 for now until we know default values (if these become non-optional)
            Preconditions.AtLeast(ContextTypes?.Count ?? 1, 1, nameof(ContextTypes), "At least 1 context type must be specified");
            Preconditions.AtLeast(IntegrationTypes?.Count ?? 1, 1, nameof(IntegrationTypes), "At least 1 integration type must be specified");

            var props = new SlashCommandProperties
            {
                Name = Name,
                Description = Description,
                IsDefaultPermission = IsDefaultPermission,
                NameLocalizations = _nameLocalizations,
                DescriptionLocalizations = _descriptionLocalizations,
#pragma warning disable CS0618 // Type or member is obsolete
                IsDMEnabled = IsDMEnabled,
#pragma warning restore CS0618 // Type or member is obsolete
                DefaultMemberPermissions = DefaultMemberPermissions ?? Optional<GuildPermission>.Unspecified,
                IsNsfw = IsNsfw,
                ContextTypes = ContextTypes ?? Optional<HashSet<InteractionContextType>>.Unspecified,
                IntegrationTypes = IntegrationTypes ?? Optional<HashSet<ApplicationIntegrationType>>.Unspecified
            };

            if (Options != null && Options.Any())
            {
                var options = new List<ApplicationCommandOptionProperties>();

                Options.OrderByDescending(x => x.IsRequired ?? false).ToList().ForEach(x => options.Add(x.Build()));

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
            Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the description of the current command.
        /// </summary>
        /// <param name="description">The description of this command.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the default permission of the current command.
        /// </summary>
        /// <param name="value">The default permission value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithDefaultPermission(bool value)
        {
            IsDefaultPermission = value;
            return this;
        }

        /// <summary>
        ///     Sets whether or not this command can be used in dms.
        /// </summary>
        /// <param name="permission"><see langword="true"/> if the command is available in dms, otherwise <see langword="false"/>.</param>
        /// <returns>The current builder.</returns>
        [Obsolete("This method will be deprecated soon. Configure using WithContextTypes instead.")]
        public SlashCommandBuilder WithDMPermission(bool permission)
        {
            IsDMEnabled = permission;
            return this;
        }

        /// <summary>
        ///     Sets whether or not this command is age restricted.
        /// </summary>
        /// <param name="permission"><see langword="true"/> if the command is age restricted, otherwise <see langword="false"/>.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithNsfw(bool permission)
        {
            IsNsfw = permission;
            return this;
        }

        /// <summary>
        ///     Sets the default member permissions required to use this application command.
        /// </summary>
        /// <param name="permissions">The permissions required to use this command.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithDefaultMemberPermissions(GuildPermission? permissions)
        {
            DefaultMemberPermissions = permissions;
            return this;
        }

        /// <summary>
        ///     Sets the installation method for this command.
        /// </summary>
        /// <param name="integrationTypes">Installation types for this command.</param>
        /// <returns>The builder instance.</returns>
        public SlashCommandBuilder WithIntegrationTypes(params ApplicationIntegrationType[] integrationTypes)
        {
            IntegrationTypes = integrationTypes is not null
                ? new HashSet<ApplicationIntegrationType>(integrationTypes)
                : null;
            return this;
        }

        /// <summary>
        ///     Sets context types this command can be executed in.
        /// </summary>
        /// <param name="contextTypes">Context types the command can be executed in.</param>
        /// <returns>The builder instance.</returns>
        public SlashCommandBuilder WithContextTypes(params InteractionContextType[] contextTypes)
        {
            ContextTypes = contextTypes is not null
                ? new HashSet<InteractionContextType>(contextTypes)
                : null;
            return this;
        }

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="isRequired">If this option is required for this command.</param>
        /// <param name="isDefault">If this option is the default option.</param>
        /// <param name="isAutocomplete">If this option is set to autocomplete.</param>
        /// <param name="options">The options of the option to add.</param>
        /// <param name="channelTypes">The allowed channel types for this option.</param>
        /// <param name="nameLocalizations">Localization dictionary for the name field of this command.</param>
        /// <param name="descriptionLocalizations">Localization dictionary for the description field of this command.</param>
        /// <param name="choices">The choices of this option.</param>
        /// <param name="minValue">The smallest number value the user can input.</param>
        /// <param name="maxValue">The largest number value the user can input.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(string name, ApplicationCommandOptionType type,
           string description, bool? isRequired = null, bool? isDefault = null, bool isAutocomplete = false, double? minValue = null, double? maxValue = null,
           List<SlashCommandOptionBuilder> options = null, List<ChannelType> channelTypes = null, IDictionary<string, string> nameLocalizations = null,
           IDictionary<string, string> descriptionLocalizations = null,
           int? minLength = null, int? maxLength = null, params ApplicationCommandOptionChoiceProperties[] choices)
        {
            Preconditions.Options(name, description);

            // https://discord.com/developers/docs/interactions/application-commands
            if (!Regex.IsMatch(name, @"^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$"))
                throw new ArgumentException(@"Name must match the regex ^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$", nameof(name));

            // make sure theres only one option with default set to true
            if (isDefault == true && Options?.Any(x => x.IsDefault == true) == true)
                throw new ArgumentException("There can only be one command option with default set to true!", nameof(isDefault));

            var option = new SlashCommandOptionBuilder
            {
                Name = name,
                Description = description,
                IsRequired = isRequired,
                IsDefault = isDefault,
                Options = options,
                Type = type,
                IsAutocomplete = isAutocomplete,
                Choices = (choices ?? Array.Empty<ApplicationCommandOptionChoiceProperties>()).ToList(),
                ChannelTypes = channelTypes,
                MinValue = minValue,
                MaxValue = maxValue,
                MinLength = minLength,
                MaxLength = maxLength,
            };

            if (nameLocalizations is not null)
                option.WithNameLocalizations(nameLocalizations);

            if (descriptionLocalizations is not null)
                option.WithDescriptionLocalizations(descriptionLocalizations);

            return AddOption(option);
        }

        /// <summary>
        ///     Adds an option to this slash command.
        /// </summary>
        /// <param name="option">The option to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(SlashCommandOptionBuilder option)
        {
            Options ??= new List<SlashCommandOptionBuilder>();

            if (Options.Count >= MaxOptionsCount)
                throw new InvalidOperationException($"Cannot have more than {MaxOptionsCount} options!");

            Preconditions.NotNull(option, nameof(option));
            Preconditions.Options(option.Name, option.Description); // this is a double-check when this method is called via AddOption(string name... )

            Options.Add(option);
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

            Options ??= new List<SlashCommandOptionBuilder>();

            if (Options.Count + options.Length > MaxOptionsCount)
                throw new ArgumentOutOfRangeException(nameof(options), $"Cannot have more than {MaxOptionsCount} options!");

            foreach (var option in options)
                Preconditions.Options(option.Name, option.Description);

            Options.AddRange(options);
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="NameLocalizations"/> collection.
        /// </summary>
        /// <param name="nameLocalizations">The localization dictionary to use for the name field of this command.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nameLocalizations"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any dictionary key is an invalid locale string.</exception>
        public SlashCommandBuilder WithNameLocalizations(IDictionary<string, string> nameLocalizations)
        {
            if (nameLocalizations is null)
                throw new ArgumentNullException(nameof(nameLocalizations));

            foreach (var (locale, name) in nameLocalizations)
            {
                if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                    throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                EnsureValidCommandName(name);
            }

            _nameLocalizations = new Dictionary<string, string>(nameLocalizations);
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="DescriptionLocalizations"/> collection.
        /// </summary>
        /// <param name="descriptionLocalizations">The localization dictionary to use for the description field of this command.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="descriptionLocalizations"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any dictionary key is an invalid locale string.</exception>
        public SlashCommandBuilder WithDescriptionLocalizations(IDictionary<string, string> descriptionLocalizations)
        {
            if (descriptionLocalizations is null)
                throw new ArgumentNullException(nameof(descriptionLocalizations));

            foreach (var (locale, description) in descriptionLocalizations)
            {
                if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                    throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                EnsureValidCommandDescription(description);
            }

            _descriptionLocalizations = new Dictionary<string, string>(descriptionLocalizations);
            return this;
        }

        /// <summary>
        ///     Adds a new entry to the <see cref="NameLocalizations"/> collection.
        /// </summary>
        /// <param name="locale">Locale of the entry.</param>
        /// <param name="name">Localized string for the name field.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locale"/> is an invalid locale string.</exception>
        public SlashCommandBuilder AddNameLocalization(string locale, string name)
        {
            if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

            EnsureValidCommandName(name);

            _nameLocalizations ??= new();
            _nameLocalizations.Add(locale, name);

            return this;
        }

        /// <summary>
        ///     Adds a new entry to the <see cref="Description"/> collection.
        /// </summary>
        /// <param name="locale">Locale of the entry.</param>
        /// <param name="description">Localized string for the description field.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locale"/> is an invalid locale string.</exception>
        public SlashCommandBuilder AddDescriptionLocalization(string locale, string description)
        {
            if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

            EnsureValidCommandDescription(description);

            _descriptionLocalizations ??= new();
            _descriptionLocalizations.Add(locale, description);

            return this;
        }

        internal static void EnsureValidCommandName(string name)
        {
            Preconditions.NotNullOrEmpty(name, nameof(name));
            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, MaxNameLength, nameof(name));

            // https://discord.com/developers/docs/interactions/application-commands
            if (!Regex.IsMatch(name, @"^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$"))
                throw new ArgumentException(@"Name must match the regex ^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$", nameof(name));

            if (name.Any(char.IsUpper))
                throw new FormatException("Name cannot contain any uppercase characters.");
        }

        internal static void EnsureValidCommandDescription(string description)
        {
            Preconditions.NotNullOrEmpty(description, nameof(description));
            Preconditions.AtLeast(description.Length, 1, nameof(description));
            Preconditions.AtMost(description.Length, MaxDescriptionLength, nameof(description));
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
        public const int MaxChoiceCount = 25;

        private string _name;
        private string _description;
        private Dictionary<string, string> _nameLocalizations;
        private Dictionary<string, string> _descriptionLocalizations;

        /// <summary>
        ///     Gets or sets the name of this option.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value != null)
                {
                    EnsureValidCommandOptionName(value);
                }

                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the description of this option.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (value != null)
                {
                    EnsureValidCommandOptionDescription(value);
                }

                _description = value;
            }
        }

        /// <summary>
        ///     Gets or sets the type of this option.
        /// </summary>
        public ApplicationCommandOptionType Type { get; set; }

        /// <summary>
        ///     Gets or sets whether or not this options is the first required option for the user to complete. only one option can be default.
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        ///     Gets or sets if the option is required.
        /// </summary>
        public bool? IsRequired { get; set; } = null;

        /// <summary>
        ///     Gets or sets whether or not this option supports autocomplete.
        /// </summary>
        public bool IsAutocomplete { get; set; }

        /// <summary>
        ///     Gets or sets the smallest number value the user can input.
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        ///     Gets or sets the largest number value the user can input.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the minimum allowed length for a string input.
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        ///     Gets or sets the maximum allowed length for a string input.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        ///     Gets or sets the choices for string and int types for the user to pick from.
        /// </summary>
        public List<ApplicationCommandOptionChoiceProperties> Choices { get; set; }

        /// <summary>
        ///     Gets or sets if this option is a subcommand or subcommand group type, these nested options will be the parameters.
        /// </summary>
        public List<SlashCommandOptionBuilder> Options { get; set; }

        /// <summary>
        ///     Gets or sets the allowed channel types for this option.
        /// </summary>
        public List<ChannelType> ChannelTypes { get; set; }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations => _nameLocalizations;

        /// <summary>
        ///     Gets the localization dictionary for the description field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> DescriptionLocalizations => _descriptionLocalizations;

        /// <summary>
        ///     Builds the current option.
        /// </summary>
        /// <returns>The built version of this option.</returns>
        public ApplicationCommandOptionProperties Build()
        {
            bool isSubType = Type == ApplicationCommandOptionType.SubCommandGroup;
            bool isIntType = Type == ApplicationCommandOptionType.Integer;
            bool isStrType = Type == ApplicationCommandOptionType.String;

            if (isSubType && (Options == null || !Options.Any()))
                throw new InvalidOperationException("SubCommands/SubCommandGroups must have at least one option");

            if (!isSubType && Options != null && Options.Any() && Type != ApplicationCommandOptionType.SubCommand)
                throw new InvalidOperationException($"Cannot have options on {Type} type");

            if (isIntType && MinValue != null && MinValue % 1 != 0)
                throw new InvalidOperationException("MinValue cannot have decimals on Integer command options.");

            if (isIntType && MaxValue != null && MaxValue % 1 != 0)
                throw new InvalidOperationException("MaxValue cannot have decimals on Integer command options.");

            if (isStrType && MinLength is not null && MinLength < 0)
                throw new InvalidOperationException("MinLength cannot be smaller than 0.");

            if (isStrType && MaxLength is not null && MaxLength < 1)
                throw new InvalidOperationException("MaxLength cannot be smaller than 1.");

            return new ApplicationCommandOptionProperties
            {
                Name = Name,
                Description = Description,
                IsDefault = IsDefault,
                IsRequired = IsRequired,
                Type = Type,
                Options = Options?.Count > 0
                    ? Options.OrderByDescending(x => x.IsRequired ?? false).Select(x => x.Build()).ToList()
                    : new List<ApplicationCommandOptionProperties>(),
                Choices = Choices,
                IsAutocomplete = IsAutocomplete,
                ChannelTypes = ChannelTypes,
                MinValue = MinValue,
                MaxValue = MaxValue,
                NameLocalizations = _nameLocalizations,
                DescriptionLocalizations = _descriptionLocalizations,
                MinLength = MinLength,
                MaxLength = MaxLength,
            };
        }

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="isRequired">If this option is required for this command.</param>
        /// <param name="isDefault">If this option is the default option.</param>
        /// <param name="isAutocomplete">If this option supports autocomplete.</param>
        /// <param name="options">The options of the option to add.</param>
        /// <param name="channelTypes">The allowed channel types for this option.</param>
        /// <param name="nameLocalizations">Localization dictionary for the description field of this command.</param>
        /// <param name="descriptionLocalizations">Localization dictionary for the description field of this command.</param>
        /// <param name="choices">The choices of this option.</param>
        /// <param name="minValue">The smallest number value the user can input.</param>
        /// <param name="maxValue">The largest number value the user can input.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddOption(string name, ApplicationCommandOptionType type,
           string description, bool? isRequired = null, bool isDefault = false, bool isAutocomplete = false, double? minValue = null, double? maxValue = null,
           List<SlashCommandOptionBuilder> options = null, List<ChannelType> channelTypes = null, IDictionary<string, string> nameLocalizations = null,
           IDictionary<string, string> descriptionLocalizations = null,
           int? minLength = null, int? maxLength = null, params ApplicationCommandOptionChoiceProperties[] choices)
        {
            Preconditions.Options(name, description);

            // https://discord.com/developers/docs/interactions/application-commands
            if (!Regex.IsMatch(name, @"^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$"))
                throw new ArgumentException(@"Name must match the regex ^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$", nameof(name));

            // make sure theres only one option with default set to true
            if (isDefault && Options?.Any(x => x.IsDefault == true) == true)
                throw new ArgumentException("There can only be one command option with default set to true!", nameof(isDefault));

            var option = new SlashCommandOptionBuilder
            {
                Name = name,
                Description = description,
                IsRequired = isRequired,
                IsDefault = isDefault,
                IsAutocomplete = isAutocomplete,
                MinValue = minValue,
                MaxValue = maxValue,
                MinLength = minLength,
                MaxLength = maxLength,
                Options = options,
                Type = type,
                Choices = (choices ?? Array.Empty<ApplicationCommandOptionChoiceProperties>()).ToList(),
                ChannelTypes = channelTypes,
            };

            if (nameLocalizations is not null)
                option.WithNameLocalizations(nameLocalizations);

            if (descriptionLocalizations is not null)
                option.WithDescriptionLocalizations(descriptionLocalizations);

            return AddOption(option);
        }
        /// <summary>
        ///     Adds a sub option to the current option.
        /// </summary>
        /// <param name="option">The sub option to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddOption(SlashCommandOptionBuilder option)
        {
            Options ??= new List<SlashCommandOptionBuilder>();

            if (Options.Count >= SlashCommandBuilder.MaxOptionsCount)
                throw new InvalidOperationException($"There can only be {SlashCommandBuilder.MaxOptionsCount} options per sub command group!");

            Preconditions.NotNull(option, nameof(option));
            Preconditions.Options(option.Name, option.Description); // double check again

            Options.Add(option);
            return this;
        }

        /// <summary>
        ///     Adds a collection of options to the current option.
        /// </summary>
        /// <param name="options">The collection of options to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddOptions(params SlashCommandOptionBuilder[] options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null!");

            Options ??= new List<SlashCommandOptionBuilder>();

            if (Options.Count + options.Length > SlashCommandBuilder.MaxOptionsCount)
                throw new ArgumentOutOfRangeException(nameof(options), $"There can only be {SlashCommandBuilder.MaxOptionsCount} options per sub command group!");

            foreach (var option in options)
                Preconditions.Options(option.Name, option.Description);

            Options.AddRange(options);
            return this;
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <param name="nameLocalizations">The localization dictionary for to use the name field of this command option choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, int value, IDictionary<string, string> nameLocalizations = null)
        {
            return AddChoiceInternal(name, value, nameLocalizations);
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <param name="nameLocalizations">The localization dictionary for to use the name field of this command option choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, string value, IDictionary<string, string> nameLocalizations = null)
        {
            return AddChoiceInternal(name, value, nameLocalizations);
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <param name="nameLocalizations">Localization dictionary for the description field of this command.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, double value, IDictionary<string, string> nameLocalizations = null)
        {
            return AddChoiceInternal(name, value, nameLocalizations);
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <param name="nameLocalizations">The localization dictionary to use for the name field of this command option choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, float value, IDictionary<string, string> nameLocalizations = null)
        {
            return AddChoiceInternal(name, value, nameLocalizations);
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <param name="nameLocalizations">The localization dictionary to use for the name field of this command option choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, long value, IDictionary<string, string> nameLocalizations = null)
        {
            return AddChoiceInternal(name, value, nameLocalizations);
        }

        private SlashCommandOptionBuilder AddChoiceInternal(string name, object value, IDictionary<string, string> nameLocalizations = null)
        {
            Choices ??= new List<ApplicationCommandOptionChoiceProperties>();

            if (Choices.Count >= MaxChoiceCount)
                throw new InvalidOperationException($"Cannot add more than {MaxChoiceCount} choices!");

            Preconditions.NotNull(name, nameof(name));
            Preconditions.NotNull(value, nameof(value));

            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, 100, nameof(name));

            if (value is string str)
            {
                Preconditions.AtLeast(str.Length, 1, nameof(value));
                Preconditions.AtMost(str.Length, 100, nameof(value));
            }

            Choices.Add(new ApplicationCommandOptionChoiceProperties
            {
                Name = name,
                Value = value,
                NameLocalizations = nameLocalizations
            });

            return this;
        }

        /// <summary>
        ///     Adds a channel type to the current option.
        /// </summary>
        /// <param name="channelType">The <see cref="ChannelType"/> to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChannelType(ChannelType channelType)
        {
            ChannelTypes ??= new List<ChannelType>();

            ChannelTypes.Add(channelType);

            return this;
        }

        /// <summary>
        ///     Sets the current builders name.
        /// </summary>
        /// <param name="name">The name to set the current option builder.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithName(string name)
        {
            Name = name;

            return this;
        }

        /// <summary>
        ///     Sets the current builders description.
        /// </summary>
        /// <param name="description">The description to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the current builders required field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithRequired(bool value)
        {
            IsRequired = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders default field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithDefault(bool value)
        {
            IsDefault = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders autocomplete field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithAutocomplete(bool value)
        {
            IsAutocomplete = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders min value field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithMinValue(double value)
        {
            MinValue = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders max value field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithMaxValue(double value)
        {
            MaxValue = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders min length field.
        /// </summary>
        /// <param name="length">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithMinLength(int length)
        {
            MinLength = length;
            return this;
        }

        /// <summary>
        ///     Sets the current builders max length field.
        /// </summary>
        /// <param name="length">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithMaxLength(int length)
        {
            MaxLength = length;
            return this;
        }

        /// <summary>
        ///     Sets the current type of this builder.
        /// </summary>
        /// <param name="type">The type to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithType(ApplicationCommandOptionType type)
        {
            Type = type;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="NameLocalizations"/> collection.
        /// </summary>
        /// <param name="nameLocalizations">The localization dictionary to use for the name field of this command option.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nameLocalizations"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any dictionary key is an invalid locale string.</exception>
        public SlashCommandOptionBuilder WithNameLocalizations(IDictionary<string, string> nameLocalizations)
        {
            if (nameLocalizations is null)
                throw new ArgumentNullException(nameof(nameLocalizations));

            foreach (var (locale, name) in nameLocalizations)
            {
                if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                    throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                EnsureValidCommandOptionName(name);
            }

            _nameLocalizations = new Dictionary<string, string>(nameLocalizations);
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="DescriptionLocalizations"/> collection.
        /// </summary>
        /// <param name="descriptionLocalizations">The localization dictionary to use for the description field of this command option.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="descriptionLocalizations"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any dictionary key is an invalid locale string.</exception>
        public SlashCommandOptionBuilder WithDescriptionLocalizations(IDictionary<string, string> descriptionLocalizations)
        {
            if (descriptionLocalizations is null)
                throw new ArgumentNullException(nameof(descriptionLocalizations));

            foreach (var (locale, description) in descriptionLocalizations)
            {
                if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                    throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                EnsureValidCommandOptionDescription(description);
            }

            _descriptionLocalizations = new Dictionary<string, string>(descriptionLocalizations);
            return this;
        }

        /// <summary>
        ///     Adds a new entry to the <see cref="NameLocalizations"/> collection.
        /// </summary>
        /// <param name="locale">Locale of the entry.</param>
        /// <param name="name">Localized string for the name field.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locale"/> is an invalid locale string.</exception>
        public SlashCommandOptionBuilder AddNameLocalization(string locale, string name)
        {
            if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

            EnsureValidCommandOptionName(name);

            _nameLocalizations ??= new();
            _nameLocalizations.Add(locale, name);

            return this;
        }

        /// <summary>
        ///     Adds a new entry to the <see cref="DescriptionLocalizations"/> collection.
        /// </summary>
        /// <param name="locale">Locale of the entry.</param>
        /// <param name="description">Localized string for the description field.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locale"/> is an invalid locale string.</exception>
        public SlashCommandOptionBuilder AddDescriptionLocalization(string locale, string description)
        {
            if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

            EnsureValidCommandOptionDescription(description);

            _descriptionLocalizations ??= new();
            _descriptionLocalizations.Add(locale, description);

            return this;
        }

        private static void EnsureValidCommandOptionName(string name)
        {
            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, SlashCommandBuilder.MaxNameLength, nameof(name));

            // https://discord.com/developers/docs/interactions/application-commands
            if (!Regex.IsMatch(name, @"^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$"))
                throw new ArgumentException(@"Name must match the regex ^[-_\p{L}\p{N}\p{IsDevanagari}\p{IsThai}]{1,32}$", nameof(name));
        }

        private static void EnsureValidCommandOptionDescription(string description)
        {
            Preconditions.AtLeast(description.Length, 1, nameof(description));
            Preconditions.AtMost(description.Length, SlashCommandBuilder.MaxDescriptionLength, nameof(description));
        }
    }
}
