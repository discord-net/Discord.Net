---
uid: Guides.Commands.PostExecution
title: Post-command Execution Handling
---

> [!WARNING]
> This page is still under construction!

# Preface

When developing a command system or modules, you may want to consider
building a post-execution handling system so you can have a finer
control over commands. Discord.Net offers several different
post-execution workflow for you to work with.

If you recall, in the [Command Guide], we've shown the following
example for executing and handling commands,

[!code[Command Handler](samples/command_handler.cs)]

You may notice that after we perform [ExecuteAsync], we store the
result and print it to the chat. This is essentially the most
basic post-execution handling. With this in mind, we could start doing
things like the following,

[!code[Basic Command Handler](samples/post-execution_basic.cs)]

**But!** This may not always be preferred, because you are
creating your post-execution logic *with* the essential command
handler. This could lead to messy code and has another potential
issue, working with `RunMode.Async`.

If your command is marked with `RunMode.Async`, [ExecuteAsync] will
return a successful [ExecuteResult] instead of whatever results
the actual command may return. Because of the way `RunMode.Async`
[works](xref:FAQ.Commands), handling within the command handler
may not always achieve the desired effect.

## CommandExecuted Event

Enter [CommandExecuted], an event that was introduced in
Discord.Net 2.0. This event is raised when the command is
sucessfully executed **without any runtime exceptions** (more on this
later). This means this event can be used to streamline your
post-execution design, and the best thing about this event is that it
is not prone to `RunMode.Async`'s [ExecuteAsync] drawbacks.

With that in mind, we can begin working on code such as:

```cs
public async Task SetupAsync()
{
    await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    // Hook the execution event
    _command.CommandExecuted += OnCommandExecutedAsync;
    // Hook the command handler
    _client.MessageReceived += HandleCommandAsync;
}
public async Task OnCommandExecutedAsync(CommandInfo command, ICommandContext context, IResult result)
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
    await _log.LogAsync(new LogMessage(LogSeverity.Info, "CommandExecution", $"{command.Name} was executed at {DateTime.UtcNow}."));
}
public async Task HandleCommandAsync(SocketMessage msg)
{
    // Notice how clean our new command handler has become.
    var message = messageParam as SocketUserMessage;
    if (message == null) return;
    int argPos = 0;
    if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
    var context = new SocketCommandContext(_client, message);
    await _commands.ExecuteAsync(context, argPos, _services);
}
```

So now we have a streamlined post-execution pipeline, great! What's
next? We can take this further by using [RuntimeResult].

### RuntimeResult

This class was introduced in 1.0, but it wasn't widely adopted due to
the previously mentioned [ExecuteAsync] drawback. Since we now have
access to proper result handling via the [CommandExecuted] event,
we can start making use of this class.

#### What is it?

`RuntimeResult` was introduced to allow developers to centralize
their command result logic. It is a result type that is designed to
be returned when the command has finished its logic.

#### How to make use of it?

The best way to make use of it is to create your own version of
`RuntimeResult`. You can achieve this by inheriting the `RuntimeResult`
class.

```cs
public class MyCustomResult : RuntimeResult
{
}
```

// todo: finish this section

## CommandService.Log Event

We have so far covered the handling of various result types, but we
haven't talked about what to do if the command enters a catastrophic
failure (i.e. exceptions). To resolve this, we can make use of the
[CommandService.Log] event.

All exceptions thrown during a command execution will be caught and
sent to the Log event under the [LogMessage.Exception] property as a
[CommandException] type. The [CommandException] class allows us to
access the exception thrown, as well as the context of the command.

```cs
public async Task LogAsync(LogMessage logMessage)
{
    // This casting type requries C#7
    if (logMessage.Exception is CommandException cmdException)
    {
        // We can tell the user that something unexpected has happened
        await cmdException.Context.Channel.SendMessageAsync("Something went catastrophically wrong!");

        // We can also log this incident
        Console.WriteLine($"{cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.");
        Console.WriteLine(cmdException.ToString());
    }
}
```

[CommandException]: xref:Discord.Commands.CommandException
[LogMessage.Exception]: xref:Discord.LogMessage.Exception
[CommandService.Log]: xref:Discord.Commands.CommandService.Log
[RuntimeResult]: xref:Discord.Commands.RuntimeResult
[CommandExecuted]: xref:Discord.Commands.CommandService.CommandExecuted
[ExecuteAsync]: xref:Discord.Commands.CommandService.ExecuteAsync*
[ExecuteResult]: xref:Discord.Commands.ExecuteResult
[Command Guide]: xref:Guides.Commands.Intro