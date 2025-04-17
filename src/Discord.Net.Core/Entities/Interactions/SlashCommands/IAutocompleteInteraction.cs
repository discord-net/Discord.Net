using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="InteractionType.ApplicationCommandAutocomplete"/>.
    /// </summary>
    public interface IAutocompleteInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the autocomplete data of this interaction.
        /// </summary>
        new IAutocompleteInteractionData Data { get; }

        /// <summary>
        ///     Responds to this interaction with a set of choices.
        /// </summary>
        /// <param name="result">
        ///     The set of choices for the user to pick from.
        ///     <remarks>
        ///         A max of 25 choices are allowed. Passing <see langword="null"/> for this argument will show the executing user that
        ///         there is no choices for their autocompleted input.
        ///     </remarks>
        /// </param>
        /// <param name="options">The request options for this response.</param>
        Task RespondAsync(IEnumerable<AutocompleteResult> result, RequestOptions options = null);
    }
}
