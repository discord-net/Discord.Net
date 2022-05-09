// you need to add `Autocomplete` attribute before parameter to add autocompletion to it
[SlashCommand("command_name", "command_description")]
public async Task ExampleCommand([Summary("parameter_name"), Autocomplete(typeof(ExampleAutocompleteHandler))] string parameterWithAutocompletion)
    => await RespondAsync($"Your choice: {parameterWithAutocompletion}");

public class ExampleAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        // Create a collection with suggestions for autocomplete
        IEnumerable<AutocompleteResult> results = new[]
        {
            new AutocompleteResult("Name1", "value111"),
            new AutocompleteResult("Name2", "value2")
        };

        // max - 25 suggestions at a time (API limit)
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}