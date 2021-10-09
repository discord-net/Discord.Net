using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Options for the <see cref="IApplicationCommand"/>, see <see href="https://discord.com/developers/docs/interactions/slash-commands#applicationcommandoption">The docs</see>.
    /// </summary>
    public interface IApplicationCommandOption
    {
        /// <summary>
        ///     The type of this <see cref="IApplicationCommandOption"/>.
        /// </summary>
        ApplicationCommandOptionType Type { get; }

        /// <summary>
        ///     The name of this command option, 1-32 character name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The description of this command option, 1-100 character description.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     The first required option for the user to complete--only one option can be default.
        /// </summary>
        bool? IsDefault { get; }

        /// <summary>
        ///     If the parameter is required or optional, default is <see langword="false"/>.
        /// </summary>
        bool? IsRequired { get; }

        /// <summary>
        ///     Choices for string and int types for the user to pick from.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOptionChoice> Choices { get; }

        /// <summary>
        ///     If the option is a subcommand or subcommand group type, this nested options will be the parameters.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOption> Options { get; }

        /// <summary>
        ///     The allowed channel types for this option.
        /// </summary>
        IReadOnlyCollection<ChannelType> ChannelTypes { get; }
    }
}
