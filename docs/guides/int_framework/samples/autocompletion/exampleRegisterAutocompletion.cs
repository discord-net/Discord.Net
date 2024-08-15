_client.AutocompleteExecuted += HandleAutocompleteExecution;

private static async Task HandleAutocompleteExecution(SocketAutocompleteInteraction arg){
    var context = new InteractionContext(_client, arg, arg.Channel);
    await _interactionService.ExecuteCommandAsync(context, null);
}
