using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
    /// <summary>
    ///     A class used to build user commands.
    /// </summary>
    public class UserCommandBuilder
    {
        /// <summary>
        ///     Returns the maximum length a commands name allowed by Discord.
        /// </summary>
        public const int MaxNameLength = 32;

        /// <summary>
        ///     Gets or sets the name of this User command.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                Preconditions.NotNullOrEmpty(value, nameof(Name));
                Preconditions.AtLeast(value.Length, 1, nameof(Name));
                Preconditions.AtMost(value.Length, MaxNameLength, nameof(Name));

                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether the command is enabled by default when the app is added to a guild.
        /// </summary>
        public bool IsDefaultPermission { get; set; } = true;

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations => _nameLocalizations;

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

        /// <summary>
        ///     Gets the installation method for this command. <see langword="null"/> if not set.
        /// </summary>
        public HashSet<ApplicationIntegrationType> IntegrationTypes { get; set; }

        /// <summary>
        ///     Gets the context types this command can be executed in. <see langword="null"/> if not set.
        /// </summary>
        public HashSet<InteractionContextType> ContextTypes { get; set; }

        private string _name;
        private Dictionary<string, string> _nameLocalizations;

        /// <summary>
        ///     Build the current builder into a <see cref="UserCommandProperties"/> class.
        /// </summary>
        /// <returns>A <see cref="UserCommandProperties"/> that can be used to create user commands.</returns>
        public UserCommandProperties Build()
        {
            var props = new UserCommandProperties
            {
                Name = Name,
                IsDefaultPermission = IsDefaultPermission,
#pragma warning disable CS0618 // Type or member is obsolete
                IsDMEnabled = IsDMEnabled,
#pragma warning restore CS0618 // Type or member is obsolete
                DefaultMemberPermissions = DefaultMemberPermissions ?? Optional<GuildPermission>.Unspecified,
                NameLocalizations = NameLocalizations,
                IsNsfw = IsNsfw,
                ContextTypes = ContextTypes,
                IntegrationTypes = IntegrationTypes
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
            Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the default permission of the current command.
        /// </summary>
        /// <param name="isDefaultPermission">The default permission value to set.</param>
        /// <returns>The current builder.</returns>
        public UserCommandBuilder WithDefaultPermission(bool isDefaultPermission)
        {
            IsDefaultPermission = isDefaultPermission;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="NameLocalizations"/> collection.
        /// </summary>
        /// <param name="nameLocalizations">The localization dictionary to use for the name field of this command.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nameLocalizations"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any dictionary key is an invalid locale string.</exception>
        public UserCommandBuilder WithNameLocalizations(IDictionary<string, string> nameLocalizations)
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
        ///     Sets whether or not this command can be used in dms.
        /// </summary>
        /// <param name="permission"><see langword="true"/> if the command is available in dms, otherwise <see langword="false"/>.</param>
        /// <returns>The current builder.</returns>
        [Obsolete("This method will be deprecated soon. Configure with WithContextTypes instead.")]
        public UserCommandBuilder WithDMPermission(bool permission)
        {
            IsDMEnabled = permission;
            return this;
        }

        /// <summary>
        ///     Sets whether or not this command is age restricted.
        /// </summary>
        /// <param name="permission"><see langword="true"/> if the command is age restricted, otherwise <see langword="false"/>.</param>
        /// <returns>The current builder.</returns>
        public UserCommandBuilder WithNsfw(bool permission)
        {
            IsNsfw = permission;
            return this;
        }

        /// <summary>
        ///     Adds a new entry to the <see cref="NameLocalizations"/> collection.
        /// </summary>
        /// <param name="locale">Locale of the entry.</param>
        /// <param name="name">Localized string for the name field.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locale"/> is an invalid locale string.</exception>
        public UserCommandBuilder AddNameLocalization(string locale, string name)
        {
            if (!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

            EnsureValidCommandName(name);

            _nameLocalizations ??= new();
            _nameLocalizations.Add(locale, name);

            return this;
        }

        private static void EnsureValidCommandName(string name)
        {
            Preconditions.NotNullOrEmpty(name, nameof(name));
            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, MaxNameLength, nameof(name));
        }

        /// <summary>
        ///     Sets the default member permissions required to use this application command.
        /// </summary>
        /// <param name="permissions">The permissions required to use this command.</param>
        /// <returns>The current builder.</returns>
        public UserCommandBuilder WithDefaultMemberPermissions(GuildPermission? permissions)
        {
            DefaultMemberPermissions = permissions;
            return this;
        }

        /// <summary>
        ///     Sets the installation method for this command.
        /// </summary>
        /// <param name="integrationTypes">Installation types for this command.</param>
        /// <returns>The builder instance.</returns>
        public UserCommandBuilder WithIntegrationTypes(params ApplicationIntegrationType[] integrationTypes)
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
        public UserCommandBuilder WithContextTypes(params InteractionContextType[] contextTypes)
        {
            ContextTypes = contextTypes is not null
                ? new HashSet<InteractionContextType>(contextTypes)
                : null;
            return this;
        }
    }
}
