[AutocompleteCommand("parameter_name", "command_name")]
public async Task Autocomplete()
{
    string userInput = (Context.Interaction as SocketAutocompleteInteraction).Data.Current.Value.ToString();

    IEnumerable<AutocompleteResult> results = new[]
    {
        new AutocompleteResult("foo", "foo_value"),
        new AutocompleteResult("bar", "bar_value"),
        new AutocompleteResult("baz", "baz_value"),
    }.Where(x => x.Name.StartsWith(userInput)); // only send suggestions that start with user's input

    // max - 25 suggestions at a time
    await (Context.Interaction as SocketAutocompleteInteraction).RespondAsync(results.Take(25));
}

// you need to add `Autocomplete` attribute before parameter to add autocompletion to it
[SlashCommand("command_name", "command_description")]
public async Task ExampleCommand([Summary("parameter_name"), Autocomplete] string parameterWithAutocompletion)
    => await RespondAsync($"Your choice: {parameterWithAutocompletion}");