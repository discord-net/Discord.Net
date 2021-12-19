[AutocompleteCommand("parameter_name", "command_name")]
public async Task Autocomplete()
{
    IEnumerable<AutocompleteResult> results;

    ...

    await (Context.Interaction as SocketAutocompleteInteraction).RespondAsync(results);
}
