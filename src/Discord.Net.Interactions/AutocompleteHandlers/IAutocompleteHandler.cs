using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represent a Autocomplete handler object that can be executed to generate parameter suggestions.
    /// </summary>
    public interface IAutocompleteHandler
    {
        /// <summary>
        ///     Gets the the underlying command service.
        /// </summary>
        InteractionService InteractionService { get; }

        /// <summary>
        ///     Will be used to generate parameter suggestions.
        /// </summary>
        /// <param name="context">Command execution context.</param>
        /// <param name="autocompleteInteraction">Autocomplete Interaction payload.</param>
        /// <param name="parameter">Parameter information of the target parameter.</param>
        /// <param name="services">Dependencies that will be used to create the module instance.</param>
        /// <returns>
        ///     A task representing the execution process. The task result contains the Autocompletion result.
        /// </returns>
        Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services);

        /// <summary>
        ///     Executes the <see cref="IAutocompleteHandler"/> with the provided context.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="autocompleteInteraction">AutocompleteInteraction payload.</param>
        /// <param name="parameter">Parameter information of the target parameter.</param>
        /// <param name="services">Dependencies that will be used to create the module instance.</param>
        /// <returns>
        ///     A task representing the execution process. The task result contains the execution result.
        /// </returns>
        Task<IResult> ExecuteAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services);
    }
}
