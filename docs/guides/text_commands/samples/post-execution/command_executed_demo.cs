public async Task SetupAsync()
{
    await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    // Hook the execution event
    _command.CommandExecuted += OnCommandExecutedAsync;
    // Hook the command handler
    _client.MessageReceived += HandleCommandAsync;
}
public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
{
    // We have access to the information of the command executed,
    // the context of the command, and the result returned from the
    // execution in this event.

    // We can tell the user what went wrong
    if (!string.IsNullOrEmpty(result?.ErrorReason))
    {
        await context.Channel.SendMessageAsync(result.ErrorReason);
    }

    // ...or even log the result (the method used should fit into
    // your existing log handler)
    var commandName = command.IsSpecified ? command.Value.Name : "A command";
    await _log.LogAsync(new LogMessage(LogSeverity.Info, 
        "CommandExecution", 
        $"{commandName} was executed at {DateTime.UtcNow}."));
}
public async Task HandleCommandAsync(SocketMessage msg)
{
    var message = messageParam as SocketUserMessage;
    if (message == null) return;
    int argPos = 0;
    if (!(message.HasCharPrefix('!', ref argPos) || 
        message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || 
        message.Author.IsBot) return;
    var context = new SocketCommandContext(_client, message);
    await _commands.ExecuteAsync(context, argPos, _services);
}
