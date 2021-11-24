using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents data for a slash commands autocomplete interaction.
    /// </summary>
    public interface IAutocompleteInteractionData : IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the name of the invoked command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        ///     Gets the id of the invoked command.
        /// </summary>
        ulong CommandId { get; }

        /// <summary>
        ///     Gets the type of the invoked command.
        /// </summary>
        ApplicationCommandType Type { get; }

        /// <summary>
        ///     Gets the version of the invoked command.
        /// </summary>
        ulong Version { get; }

        /// <summary>
        ///     Gets the current autocomplete option that is actively being filled out.
        /// </summary>
        AutocompleteOption Current { get; }

        /// <summary>
        ///     Gets a collection of all the other options the executing users has filled out.
        /// </summary>
        IReadOnlyCollection<AutocompleteOption> Options { get; }
    }
}
