---
uid: Guides.Commands.PostExecution
title: Post-command Execution Handling
---

# Post-execution Handling for Commands

When developing commands, you may want to consider building a
post-execution handling system so you can have finer control
over commands. Discord.Net offers several post-execution workflows
for you to work with.

If you recall, in the [Command Guide], we have shown the following
example for executing and handling commands,

[!code[Command Handler](samples/intro/command_handler.cs)]

You may notice that after we perform [ExecuteAsync], we store the
result and print it to the chat, essentially creating the most
fundamental form of a post-execution handler.

With this in mind, we could start doing things like the following,

[!code[Basic Command Handler](samples/post-execution/post-execution_basic.cs)]

However, this may not always be preferred, because you are
creating your post-execution logic *with* the essential command
handler. This design could lead to messy code and could potentially
be a violation of the SRP (Single Responsibility Principle).

Another major issue is if your command is marked with
`RunMode.Async`, [ExecuteAsync] will **always** return a successful
[ExecuteResult] instead of the actual result. You can learn more
about the impact in @FAQ.Commands.General.

## CommandExecuted Event

Enter [CommandExecuted], an event that was introduced in
Discord.Net 2.0. This event is raised whenever a command is
executed regardless of its execution status. This means this 
event can be used to streamline your post-execution design, and the
best thing about this event is that it is not prone
to `RunMode.Async`'s [ExecuteAsync] drawbacks.

Thus, we can begin working on code such as:

[!code[CommandExecuted demo](samples/post-execution/command_executed_demo.cs)]

So now we have a streamlined post-execution pipeline, great! What's
next? We can take this further by using [RuntimeResult].

### RuntimeResult

`RuntimeResult` was initially introduced in 1.0 to allow
developers to centralize their command result logic.
In other words, it is a result type that is designed to be
returned when the command has finished its execution.

However, it wasn't widely adopted due to the aforementioned
[ExecuteAsync] drawback. Since we now have access to a proper
result-handler via the [CommandExecuted] event, we can start
making use of this class.

The best way to make use of it is to create your version of
`RuntimeResult`. You can achieve this by inheriting the `RuntimeResult`
class.

The following creates a bare-minimum required for a sub-class
of `RuntimeResult`,

[!code[Base Use](samples/post-execution/customresult_base.cs)]

The sky is the limit from here. You can add any additional information
you would like regarding the execution result.

For example, you may want to add your result type or other
helpful information regarding the execution, or something
simple like static methods to help you create return types easily.

[!code[Extended Use](samples/post-execution/customresult_extended.cs)]

After you're done creating your [RuntimeResult], you can
implement it in your command by marking the command return type to
`Task<RuntimeResult>`.

> [!NOTE]
> You must mark the return type as `Task<RuntimeResult>` instead of
> `Task<MyCustomResult>`. Only the former will be picked up when
> building the module.

Here's an example of a command that utilizes such logic:

[!code[Usage](samples/post-execution/customresult_usage.cs)]

And now we can check for it in our [CommandExecuted] handler:

[!code[Usage](samples/post-execution/command_executed_adv_demo.cs)]

## CommandService.Log Event

We have so far covered the handling of various result types, but we
have not talked about what to do if the command enters a catastrophic
failure (i.e., exceptions). To resolve this, we can make use of the
[CommandService.Log] event.

All exceptions thrown during a command execution are caught and sent
to the Log event under the [LogMessage.Exception] property
as a [CommandException] type. The [CommandException] class allows
us to access the exception thrown, as well as the context
of the command.

[!code[Logger Sample](samples/post-execution/command_exception_log.cs)]

[CommandException]: xref:Discord.Commands.CommandException
[LogMessage.Exception]: xref:Discord.LogMessage.Exception
[CommandService.Log]: xref:Discord.Commands.CommandService.Log
[RuntimeResult]: xref:Discord.Commands.RuntimeResult
[CommandExecuted]: xref:Discord.Commands.CommandService.CommandExecuted
[ExecuteAsync]: xref:Discord.Commands.CommandService.ExecuteAsync*
[ExecuteResult]: xref:Discord.Commands.ExecuteResult
[Command Guide]: xref:Guides.Commands.Intro