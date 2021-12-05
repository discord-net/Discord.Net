using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a configuration class for <see cref="InteractionService"/>.
    /// </summary>
    public class InteractionServiceConfig
    {
        /// <summary>
        ///     Gets or sets the minimum log level severity that will be sent to the <see cref="InteractionService.Log"/> event.
        /// </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary>
        ///     Gets or sets the default <see cref="RunMode" /> commands should have, if one is not specified on the
        ///     Command attribute or builder.
        /// </summary>
        public RunMode DefaultRunMode { get; set; } = RunMode.Async;

        /// <summary>
        ///     Gets or sets whether <see cref="RunMode.Sync"/> commands should push exceptions up to the caller.
        /// </summary>
        public bool ThrowOnError { get; set; } = true;

        /// <summary>
        ///     Gets or sets the delimiters that will be used to seperate group names and the method name when a Message Component Interaction is recieved.
        /// </summary>
        public char[] InteractionCustomIdDelimiters { get; set; }

        /// <summary>
        ///     Gets or sets the string expression that will be treated as a wild card.
        /// </summary>
        public string WildCardExpression { get; set; }

        /// <summary>
        ///     Gets or sets the option to delete Slash Command acknowledgements if no Slash Command handler is found in the <see cref="InteractionService"/>.
        /// </summary>
        public bool DeleteUnknownSlashCommandAck { get; set; } = true;

        /// <summary>
        ///     Gets or sets the option to use compiled lambda expressions to create module instances and execute commands. This method improves performance at the cost of memory.
        /// </summary>
        public bool UseCompiledLambda { get; set; } = false;

        /// <summary>
        ///     Gets or sets the option allowing you to use <see cref="AutocompleteHandler"/>s.
        /// </summary>
        /// <remarks>
        ///     Since <see cref="AutocompleteHandler"/>s are prioritized over <see cref="AutocompleteCommandInfo"/>s, if <see cref="AutocompleteHandler"/>s are not used, this should be
        ///     disabled to decrease the lookup time.
        /// </remarks>
        public bool EnableAutocompleteHandlers { get; set; } = true;

        /// <summary>
        ///     Gets or sets delegate to be used by the <see cref="InteractionService"/> when responding to a Rest based interaction.
        /// </summary>
        public RestResponseCallback RestResponseCallback { get; set; } = (ctx, str) => Task.CompletedTask;
    }

    /// <summary>
    ///     Represents a cached delegate for creating interaction responses to webhook based Discord Interactions.
    /// </summary>
    /// <param name="context">Execution context that will be injected into the module class.</param>
    /// <param name="responseBody">Body of the interaction response.</param>
    /// <returns>
    ///     A task representing the response operation.
    /// </returns>
    public delegate Task RestResponseCallback(IInteractionContext context, string responseBody);
}
