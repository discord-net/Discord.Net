using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
    /// <summary>
    ///     Represents the base class to create/modify application commands.
    /// </summary>
    public abstract class ApplicationCommandProperties
    {
        private IReadOnlyDictionary<string, string> _nameLocalizations;
        private IReadOnlyDictionary<string, string> _descriptionLocalizations;

        internal abstract ApplicationCommandType Type { get; }

        /// <summary>
        ///     Gets or sets the name of this command.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        ///     Gets or sets whether the command is enabled by default when the app is added to a guild. Default is <see langword="true"/>
        /// </summary>
        public Optional<bool> IsDefaultPermission { get; set; }

        /// <summary>
        ///     Gets or sets the localization dictionary for the name field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations
        {
            get => _nameLocalizations;
            set
            {
                foreach (var (locale, name) in value)
                {
                    if(!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                        throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                    Preconditions.AtLeast(name.Length, 1, nameof(name));
                    Preconditions.AtMost(name.Length, SlashCommandBuilder.MaxNameLength, nameof(name));
                    if (!Regex.IsMatch(name, @"^[\w-]{1,32}$"))
                        throw new ArgumentException("Option name cannot contain any special characters or whitespaces!", nameof(name));
                }
                _nameLocalizations = value;
            }
        }

        /// <summary>
        ///     Gets or sets the localization dictionary for the description field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> DescriptionLocalizations
        {
            get => _descriptionLocalizations;
            set
            {
                foreach (var (locale, description) in value)
                {
                    if(!Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                        throw new ArgumentException($"Invalid locale: {locale}", nameof(locale));

                    Preconditions.AtLeast(description.Length, 1, nameof(description));
                    Preconditions.AtMost(description.Length, SlashCommandBuilder.MaxDescriptionLength, nameof(description));
                }
                _descriptionLocalizations = value;
            }
        }

        ///     Gets or sets whether or not this command can be used in DMs.
        /// </summary>
        public Optional<bool> IsDMEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the default permissions required by a user to execute this application command.
        /// </summary>
        public Optional<GuildPermission> DefaultMemberPermissions { get; set; }

        internal ApplicationCommandProperties() { }
    }
}
