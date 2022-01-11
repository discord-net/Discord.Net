using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Options for the <see cref="IApplicationCommand"/>.
    /// </summary>
    public interface IApplicationCommandOption
    {
        /// <summary>
        ///     Gets the type of this <see cref="IApplicationCommandOption"/>.
        /// </summary>
        ApplicationCommandOptionType Type { get; }

        /// <summary>
        ///     Gets the name of this command option.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the description of this command option.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Gets whether or not this is the first required option for the user to complete.
        /// </summary>
        bool? IsDefault { get; }

        /// <summary>
        ///     Gets whether or not the parameter is required or optional.
        /// </summary>
        bool? IsRequired { get; }

        /// <summary>
        ///     Gets whether or not the option has autocomplete enabled.
        /// </summary>
        bool? IsAutocomplete { get; }

        /// <summary>
        ///     Gets the smallest number value the user can input.
        /// </summary>
        double? MinValue { get; }

        /// <summary>
        ///     Gets the largest number value the user can input.
        /// </summary>
        double? MaxValue { get; }

        /// <summary>
        ///     Gets the choices for string and int types for the user to pick from.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOptionChoice> Choices { get; }

        /// <summary>
        ///     Gets the sub-options for this command option.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOption> Options { get; }

        /// <summary>
        ///     Gets the allowed channel types for this option.
        /// </summary>
        IReadOnlyCollection<ChannelType> ChannelTypes { get; }
    }
}
