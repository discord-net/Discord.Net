using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base class for creating Autocompleters. <see cref="InteractionService"/> uses Autocompleters to generate parameter suggestions.
    /// </summary>
    public abstract class AutocompleteHandler : IAutocompleteHandler
    {
        /// <inheritdoc/>
        public InteractionService InteractionService { get; set; }

        /// <inheritdoc/>
        public abstract Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services);

        protected virtual string GetLogString(IInteractionContext context)
        {
            var interaction = (context.Interaction as IAutocompleteInteraction);
            return $"{interaction.Data.CommandName}: {interaction.Data.Current.Name} Autocomplete";
        }

        /// <inheritdoc/>
        public async Task<IResult> ExecuteAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services)
        {
            switch (InteractionService._runMode)
            {
                case RunMode.Sync:
                    {
                        return await ExecuteInternalAsync(context, autocompleteInteraction, parameter, services).ConfigureAwait(false);
                    }
                case RunMode.Async:
                    _ = Task.Run(async () =>
                    {
                        await ExecuteInternalAsync(context, autocompleteInteraction, parameter, services).ConfigureAwait(false);
                    });
                    break;
                default:
                    throw new InvalidOperationException($"RunMode {InteractionService._runMode} is not supported.");
            }

            return ExecuteResult.FromSuccess();
        }

        private async Task<IResult> ExecuteInternalAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
            IServiceProvider services)
        {
            try
            {
                var result = await GenerateSuggestionsAsync(context, autocompleteInteraction, parameter, services).ConfigureAwait(false);

                if (result.IsSuccess)
                    switch (autocompleteInteraction)
                    {
                        case RestAutocompleteInteraction restAutocomplete:
                            var payload = restAutocomplete.Respond(result.Suggestions);
                            await InteractionService._restResponseCallback(context, payload).ConfigureAwait(false);
                            break;
                        case SocketAutocompleteInteraction socketAutocomplete:
                            await socketAutocomplete.RespondAsync(result.Suggestions).ConfigureAwait(false);
                            break;
                    }

                await InteractionService._autocompleteHandlerExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                var originalEx = ex;
                while (ex is TargetInvocationException)
                    ex = ex.InnerException;

                await InteractionService._cmdLogger.ErrorAsync(ex).ConfigureAwait(false);

                var result = ExecuteResult.FromError(ex);
                await InteractionService._autocompleteHandlerExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);

                if (InteractionService._throwOnError)
                {
                    if (ex == originalEx)
                        throw;
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                return result;
            }
            finally
            {
                await InteractionService._cmdLogger.VerboseAsync($"Executed {GetLogString(context)}").ConfigureAwait(false);
            }
        }
    }
}


