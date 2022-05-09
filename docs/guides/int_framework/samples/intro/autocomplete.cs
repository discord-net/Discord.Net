[AutocompleteCommand("parameter_name", "command_name")]
public async Task Autocomplete()
{
    IEnumerable<AutocompleteResult> results = new[] 
    { 
        new AutocompleteResult("Name1", "value1"), 
        new AutocompleteResult("Name2", "value2") 
    };

        // max - 25 suggestions at a time
    await (Context.Interaction as SocketAutocompleteInteraction).RespondAsync(results.Take(25));
}

// you need to add `Autocomplete` attribute before parameter to add autocompletion to it
[SlashCommand("command_name", "command_description")]
public async Task ExampleCommand([Summary("parameter_name"), Autocomplete] string parameterWithAutocompletion)
    => await RespondAsync($"Your choice: {parameterWithAutocompletion}");